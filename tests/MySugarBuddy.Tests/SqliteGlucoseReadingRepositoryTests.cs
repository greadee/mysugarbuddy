using MySugarBuddy.Domain;
using MySugarBuddy.Infrastructure;

namespace MySugarBuddy.Tests;

public class SqliteGlucoseReadingRepositoryTests
{
    [Fact]
    public void LoadReadingsReturnsEmptyListWhenDatabaseDoesNotExist()
    {
        var repository = new SqliteGlucoseReadingRepository(CreateTempDatabasePath());

        var readings = repository.LoadReadings();

        Assert.Empty(readings);
    }

    [Fact]
    public void SaveReadingsCreatesDatabaseFile()
    {
        var databasePath = CreateTempDatabasePath();
        var repository = new SqliteGlucoseReadingRepository(databasePath);

        repository.SaveReadings(CreateReadings());

        Assert.True(File.Exists(databasePath));
    }

    [Fact]
    public void LoadReadingsReturnsSavedReadings()
    {
        var repository = new SqliteGlucoseReadingRepository(CreateTempDatabasePath());
        var readings = CreateReadings();

        repository.SaveReadings(readings);
        var loadedReadings = repository.LoadReadings();

        Assert.Equal(readings.Count, loadedReadings.Count);
        Assert.Equal(readings[0].ValueMgPerDl, loadedReadings[0].ValueMgPerDl);
        Assert.Equal(readings[0].RecordedAt, loadedReadings[0].RecordedAt);
        Assert.Equal(readings[1].ValueMgPerDl, loadedReadings[1].ValueMgPerDl);
        Assert.Equal(readings[1].RecordedAt, loadedReadings[1].RecordedAt);
    }

    [Fact]
    public void SaveReadingsAppendsNewReadings()
    {
        var repository = new SqliteGlucoseReadingRepository(CreateTempDatabasePath());
        var newReading = new GlucoseReading(140, new DateTime(2026, 1, 15, 9, 0, 0));

        repository.SaveReadings(CreateReadings());
        repository.SaveReadings(new[] { newReading });
        var loadedReadings = repository.LoadReadings();

        Assert.Equal(3, loadedReadings.Count);
        Assert.Equal(100, loadedReadings[0].ValueMgPerDl);
        Assert.Equal(115, loadedReadings[1].ValueMgPerDl);
        Assert.Equal(newReading.ValueMgPerDl, loadedReadings[2].ValueMgPerDl);
        Assert.Equal(newReading.RecordedAt, loadedReadings[2].RecordedAt);
    }

    [Fact]
    public void SaveReadingsUpdatesDuplicateTimestamp()
    {
        var repository = new SqliteGlucoseReadingRepository(CreateTempDatabasePath());
        var updatedReading = new GlucoseReading(125, new DateTime(2026, 1, 15, 8, 5, 0));

        repository.SaveReadings(CreateReadings());
        repository.SaveReadings(new[] { updatedReading });
        var loadedReadings = repository.LoadReadings();

        Assert.Equal(2, loadedReadings.Count);
        Assert.Equal(100, loadedReadings[0].ValueMgPerDl);
        Assert.Equal(updatedReading.ValueMgPerDl, loadedReadings[1].ValueMgPerDl);
        Assert.Equal(updatedReading.RecordedAt, loadedReadings[1].RecordedAt);
    }

    [Fact]
    public void SaveReadingsDoesNotDuplicateTimestampsInSameBatch()
    {
        var repository = new SqliteGlucoseReadingRepository(CreateTempDatabasePath());
        var recordedAt = new DateTime(2026, 1, 15, 8, 0, 0);
        var readings = new[]
        {
            new GlucoseReading(100, recordedAt),
            new GlucoseReading(105, recordedAt)
        };

        repository.SaveReadings(readings);
        var loadedReadings = repository.LoadReadings();

        Assert.Single(loadedReadings);
        Assert.Equal(105, loadedReadings[0].ValueMgPerDl);
        Assert.Equal(recordedAt, loadedReadings[0].RecordedAt);
    }

    private static string CreateTempDatabasePath()
    {
        return Path.Combine(Path.GetTempPath(), "mysugarbuddy-tests", $"{Guid.NewGuid()}.db");
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
