using System.Globalization;
using System.Text.Json;
using MySugarBuddy.Application;
using MySugarBuddy.Domain;

namespace MySugarBuddy.Infrastructure;

public class FileGlucoseReadingSource : IGlucoseReadingSource
{
    private readonly string _filePath;

    public FileGlucoseReadingSource(string filePath)
    {
        _filePath = filePath;
    }

    public IReadOnlyList<GlucoseReading> GetRecentReadings()
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException("Could not find glucose readings file.", _filePath);
        }

        var extension = Path.GetExtension(_filePath).ToLowerInvariant();

        var readings = extension switch
        {
            ".json" => LoadJsonReadings(),
            ".csv" => LoadCsvReadings(),
            _ => throw new NotSupportedException("Glucose readings must come from a .json or .csv file.")
        };

        return readings
            .OrderBy(reading => reading.RecordedAt)
            .ToList();
    }

    private IReadOnlyList<GlucoseReading> LoadJsonReadings()
    {
        var json = File.ReadAllText(_filePath);
        var storedReadings = JsonSerializer.Deserialize<List<StoredGlucoseReading>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<StoredGlucoseReading>();

        return storedReadings
            .Select(reading => new GlucoseReading(reading.ValueMgPerDl, reading.RecordedAt))
            .ToList();
    }

    private IReadOnlyList<GlucoseReading> LoadCsvReadings()
    {
        var lines = File.ReadAllLines(_filePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        if (lines.Count == 0)
        {
            return Array.Empty<GlucoseReading>();
        }

        var headers = lines[0].Split(',').Select(header => header.Trim()).ToList();
        var recordedAtIndex = headers.FindIndex(header => header.Equals("RecordedAt", StringComparison.OrdinalIgnoreCase));
        var valueIndex = headers.FindIndex(header => header.Equals("ValueMgPerDl", StringComparison.OrdinalIgnoreCase));

        if (recordedAtIndex < 0 || valueIndex < 0)
        {
            throw new FormatException("CSV glucose readings need RecordedAt and ValueMgPerDl columns.");
        }

        return lines
            .Skip(1)
            .Select(line => ParseCsvReading(line, recordedAtIndex, valueIndex))
            .ToList();
    }

    private static GlucoseReading ParseCsvReading(string line, int recordedAtIndex, int valueIndex)
    {
        var values = line.Split(',').Select(value => value.Trim()).ToList();

        if (values.Count <= recordedAtIndex || values.Count <= valueIndex)
        {
            throw new FormatException("CSV glucose reading row is missing a required value.");
        }

        var recordedAt = DateTime.Parse(values[recordedAtIndex], CultureInfo.InvariantCulture);
        var valueMgPerDl = int.Parse(values[valueIndex], CultureInfo.InvariantCulture);

        return new GlucoseReading(valueMgPerDl, recordedAt);
    }

    private record StoredGlucoseReading(int ValueMgPerDl, DateTime RecordedAt);
}
