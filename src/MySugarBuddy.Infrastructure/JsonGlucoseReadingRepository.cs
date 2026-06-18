using System.Text.Json;
using MySugarBuddy.Application;
using MySugarBuddy.Domain;

namespace MySugarBuddy.Infrastructure;

public class JsonGlucoseReadingRepository : IGlucoseReadingRepository
{
    private readonly string _filePath;

    public JsonGlucoseReadingRepository(string filePath)
    {
        _filePath = filePath;
    }

    public void SaveReadings(IReadOnlyList<GlucoseReading> readings)
    {
        var folderPath = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var storedReadings = readings
            .Select(reading => new StoredGlucoseReading(reading.ValueMgPerDl, reading.RecordedAt))
            .ToList();

        var json = JsonSerializer.Serialize(storedReadings, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_filePath, json);
    }

    public IReadOnlyList<GlucoseReading> LoadReadings()
    {
        if (!File.Exists(_filePath))
        {
            return Array.Empty<GlucoseReading>();
        }

        var json = File.ReadAllText(_filePath);
        var storedReadings = JsonSerializer.Deserialize<List<StoredGlucoseReading>>(json) ?? new List<StoredGlucoseReading>();

        return storedReadings
            .Select(reading => new GlucoseReading(reading.ValueMgPerDl, reading.RecordedAt))
            .ToList();
    }

    private record StoredGlucoseReading(int ValueMgPerDl, DateTime RecordedAt);
}
