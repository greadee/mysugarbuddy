using MySugarBuddy.Domain;
using MySugarBuddy.Infrastructure;

namespace MySugarBuddy.Tests;

public class JsonGlucoseReadingRepositoryTests
{
    [Fact]
    public void LoadReadingsReturnsEmptyListWhenFileDoesNotExist()
    {
        var repository = new JsonGlucoseReadingRepository(CreateTempFilePath());

        var readings = repository.LoadReadings();

        Assert.Empty(readings);
    }

    [Fact]
    public void SaveReadingsCreatesJsonFile()
    {
        var filePath = CreateTempFilePath();
        var repository = new JsonGlucoseReadingRepository(filePath);

        repository.SaveReadings(CreateReadings());

        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void LoadReadingsReturnsSavedReadings()
    {
        var repository = new JsonGlucoseReadingRepository(CreateTempFilePath());
        var readings = CreateReadings();

        repository.SaveReadings(readings);
        var loadedReadings = repository.LoadReadings();

        Assert.Equal(readings.Count, loadedReadings.Count);
        Assert.Equal(readings[0].ValueMgPerDl, loadedReadings[0].ValueMgPerDl);
        Assert.Equal(readings[0].RecordedAt, loadedReadings[0].RecordedAt);
        Assert.Equal(readings[1].ValueMgPerDl, loadedReadings[1].ValueMgPerDl);
        Assert.Equal(readings[1].RecordedAt, loadedReadings[1].RecordedAt);
    }

    private static string CreateTempFilePath()
    {
        return Path.Combine(Path.GetTempPath(), "mysugarbuddy-tests", $"{Guid.NewGuid()}.json");
    }

    private static IReadOnlyList<GlucoseReading> CreateReadings()
    {
        var startTime = new DateTime(2026, 1, 15, 8, 0, 0);

        return new[]
        {
            new GlucoseReading(100, startTime),
            new GlucoseReading(115, startTime.AddMinutes(5))
        };
    }
}
