using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using MySugarBuddy.Application;
using MySugarBuddy.Domain;

namespace MySugarBuddy.Infrastructure;

public class DexcomGlucoseReadingSource : IGlucoseReadingSource
{
    private readonly HttpClient _httpClient;
    private readonly DexcomGlucoseReadingSourceOptions _options;

    public DexcomGlucoseReadingSource(HttpClient httpClient, DexcomGlucoseReadingSourceOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public IReadOnlyList<GlucoseReading> GetRecentReadings()
    {
        if (string.IsNullOrWhiteSpace(_options.AccessToken))
        {
            throw new InvalidOperationException("Dexcom access token is required.");
        }

        var endTime = _options.UtcNow();
        var startTime = endTime.Subtract(_options.Lookback);
        var requestUri = BuildRequestUri(startTime, endTime);

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);

        using var response = _httpClient.Send(request);
        response.EnsureSuccessStatusCode();

        using var stream = response.Content.ReadAsStream();
        var dexcomResponse = JsonSerializer.Deserialize<DexcomEgvResponse>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (dexcomResponse is null)
        {
            return Array.Empty<GlucoseReading>();
        }

        return dexcomResponse.Records
            .Where(record => record.Value > 0)
            .Select(record => new GlucoseReading(record.Value, record.RecordedAt))
            .OrderBy(reading => reading.RecordedAt)
            .ToList();
    }

    private Uri BuildRequestUri(DateTime startTime, DateTime endTime)
    {
        var baseUri = _options.BaseUri.ToString().TrimEnd('/');
        var startDate = Uri.EscapeDataString(FormatDexcomDate(startTime));
        var endDate = Uri.EscapeDataString(FormatDexcomDate(endTime));

        return new Uri($"{baseUri}/v3/users/self/egvs?startDate={startDate}&endDate={endDate}");
    }

    private static string FormatDexcomDate(DateTime dateTime)
    {
        return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
    }

    private class DexcomEgvResponse
    {
        public IReadOnlyList<DexcomEgvRecord> Records { get; init; } = Array.Empty<DexcomEgvRecord>();
    }

    private class DexcomEgvRecord
    {
        public int Value { get; init; }

        [JsonPropertyName("systemTime")]
        public DateTime RecordedAt { get; init; }
    }
}
