using MySugarBuddy.Infrastructure;

namespace MySugarBuddy.Tests;

public class FileGlucoseReadingSourceTests
{
    [Fact]
    public void LoadsJsonReadings()
    {
        var filePath = CreateTempFilePath("json");
        File.WriteAllText(filePath, """
            [
              { "valueMgPerDl": 118, "recordedAt": "2026-01-15T08:05:00" },
              { "valueMgPerDl": 105, "recordedAt": "2026-01-15T08:00:00" }
            ]
            """);

        var source = new FileGlucoseReadingSource(filePath);

        var readings = source.GetRecentReadings();

        Assert.Equal(2, readings.Count);
        Assert.Equal(105, readings[0].ValueMgPerDl);
        Assert.Equal(118, readings[1].ValueMgPerDl);
    }

    [Fact]
    public void LoadsCsvReadings()
    {
        var filePath = CreateTempFilePath("csv");
        File.WriteAllText(filePath, """
            RecordedAt,ValueMgPerDl
            2026-01-15T08:05:00,118
            2026-01-15T08:00:00,105
            """);

        var source = new FileGlucoseReadingSource(filePath);

        var readings = source.GetRecentReadings();

        Assert.Equal(2, readings.Count);
        Assert.Equal(105, readings[0].ValueMgPerDl);
        Assert.Equal(118, readings[1].ValueMgPerDl);
    }

    [Fact]
    public void ReturnsReadingsInRecordedTimeOrder()
    {
        var filePath = CreateTempFilePath("csv");
        File.WriteAllText(filePath, """
            ValueMgPerDl,RecordedAt
            122,2026-01-15T08:10:00
            105,2026-01-15T08:00:00
            118,2026-01-15T08:05:00
            """);

        var source = new FileGlucoseReadingSource(filePath);

        var readings = source.GetRecentReadings();

        Assert.Equal(105, readings[0].ValueMgPerDl);
        Assert.Equal(118, readings[1].ValueMgPerDl);
        Assert.Equal(122, readings[2].ValueMgPerDl);
    }

    private static string CreateTempFilePath(string extension)
    {
        var folderPath = Path.Combine(Path.GetTempPath(), "mysugarbuddy-tests");
        Directory.CreateDirectory(folderPath);

        return Path.Combine(folderPath, $"{Guid.NewGuid()}.{extension}");
    }
}
