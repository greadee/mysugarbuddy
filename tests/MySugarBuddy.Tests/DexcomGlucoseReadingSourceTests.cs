using System.Net;
using MySugarBuddy.Infrastructure;

namespace MySugarBuddy.Tests;

public class DexcomGlucoseReadingSourceTests
{
    private readonly DateTime _now = new(2026, 1, 15, 11, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void GetRecentReadingsMapsDexcomRecordsToDomainReadings()
    {
        var handler = new FakeHttpMessageHandler("""
            {
              "records": [
                { "systemTime": "2026-01-15T10:55:00", "value": 118 },
                { "systemTime": "2026-01-15T10:50:00", "value": 105 }
              ]
            }
            """);
        var source = CreateSource(handler);

        var readings = source.GetRecentReadings();

        Assert.Equal(2, readings.Count);
        Assert.Equal(105, readings[0].ValueMgPerDl);
        Assert.Equal(new DateTime(2026, 1, 15, 10, 50, 0), readings[0].RecordedAt);
        Assert.Equal(118, readings[1].ValueMgPerDl);
    }

    [Fact]
    public void GetRecentReadingsCallsDexcomEgvEndpointWithBearerToken()
    {
        var handler = new FakeHttpMessageHandler("""
            { "records": [] }
            """);
        var source = CreateSource(handler);

        source.GetRecentReadings();

        Assert.Equal("Bearer", handler.Request?.Headers.Authorization?.Scheme);
        Assert.Equal("test-token", handler.Request?.Headers.Authorization?.Parameter);
        Assert.Equal("/v3/users/self/egvs", handler.Request?.RequestUri?.AbsolutePath);
        Assert.Contains("startDate=2026-01-15T08%3A00%3A00", handler.Request?.RequestUri?.Query);
        Assert.Contains("endDate=2026-01-15T11%3A00%3A00", handler.Request?.RequestUri?.Query);
    }

    [Fact]
    public void GetRecentReadingsSkipsRecordsWithoutPositiveGlucoseValue()
    {
        var handler = new FakeHttpMessageHandler("""
            {
              "records": [
                { "systemTime": "2026-01-15T10:50:00", "value": 0 },
                { "systemTime": "2026-01-15T10:55:00", "value": 118 }
              ]
            }
            """);
        var source = CreateSource(handler);

        var readings = source.GetRecentReadings();

        var reading = Assert.Single(readings);
        Assert.Equal(118, reading.ValueMgPerDl);
    }

    [Fact]
    public void GetRecentReadingsRequiresAccessToken()
    {
        var source = new DexcomGlucoseReadingSource(
            new HttpClient(new FakeHttpMessageHandler("{ \"records\": [] }")),
            new DexcomGlucoseReadingSourceOptions
            {
                AccessToken = "",
                BaseUri = new Uri("https://example.test")
            });

        Assert.Throws<InvalidOperationException>(() => source.GetRecentReadings());
    }

    private DexcomGlucoseReadingSource CreateSource(FakeHttpMessageHandler handler)
    {
        return new DexcomGlucoseReadingSource(
            new HttpClient(handler),
            new DexcomGlucoseReadingSourceOptions
            {
                AccessToken = "test-token",
                BaseUri = new Uri("https://example.test"),
                Lookback = TimeSpan.FromHours(3),
                UtcNow = () => _now
            });
    }

    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _json;

        public FakeHttpMessageHandler(string json)
        {
            _json = json;
        }

        public HttpRequestMessage? Request { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request, cancellationToken));
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_json)
            };
        }
    }
}
