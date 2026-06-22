using MySugarBuddy.Application;
using MySugarBuddy.Domain;

namespace MySugarBuddy.Tests;

public class GlucoseReadingServiceTests
{
    private readonly DateTime _startTime = new(2026, 1, 15, 8, 0, 0);

    [Fact]
    public void RefreshReadingsSavesRecentReadings()
    {
        var readings = CreateFastRisingReadings();
        var repository = new FakeGlucoseReadingRepository();
        var service = CreateService(readings, repository);

        service.RefreshReadings();

        Assert.Equal(readings.Count, repository.SavedReadings.Count);
    }

    [Fact]
    public void RefreshReadingsReturnsSummaryForSavedReadings()
    {
        var readings = CreateFastRisingReadings();
        var repository = new FakeGlucoseReadingRepository();
        var service = CreateService(readings, repository);

        var snapshot = service.RefreshReadings();

        Assert.Equal(2, snapshot.Summary.ReadingCount);
        Assert.Equal(105, snapshot.Summary.LowestValueMgPerDl);
        Assert.Equal(118, snapshot.Summary.HighestValueMgPerDl);
        Assert.Equal(111.5, snapshot.Summary.AverageValueMgPerDl);
    }

    [Fact]
    public void RefreshReadingsReturnsAlertsForSavedReadings()
    {
        var readings = CreateFastRisingReadings();
        var repository = new FakeGlucoseReadingRepository();
        var service = CreateService(readings, repository);

        var snapshot = service.RefreshReadings();

        Assert.Contains(GlucoseAlertType.RisingFast, snapshot.Alerts);
    }

    [Fact]
    public void RefreshReadingsReturnsCurrentTrend()
    {
        var readings = CreateFastRisingReadings();
        var repository = new FakeGlucoseReadingRepository();
        var service = CreateService(readings, repository);

        var snapshot = service.RefreshReadings();

        Assert.Equal(GlucoseTrend.RisingFast, snapshot.CurrentTrend);
    }

    [Fact]
    public void RefreshReadingsReturnsNoAlertsWhenOnlyOneReadingExists()
    {
        var readings = new[]
        {
            new GlucoseReading(105, _startTime)
        };
        var repository = new FakeGlucoseReadingRepository();
        var service = CreateService(readings, repository);

        var snapshot = service.RefreshReadings();

        Assert.Empty(snapshot.Alerts);
        Assert.Null(snapshot.CurrentTrend);
    }

    private GlucoseReadingService CreateService(
        IReadOnlyList<GlucoseReading> readings,
        FakeGlucoseReadingRepository repository)
    {
        return new GlucoseReadingService(
            new FakeGlucoseReadingSource(readings),
            repository,
            new GlucoseAlertService());
    }

    private IReadOnlyList<GlucoseReading> CreateFastRisingReadings()
    {
        return new[]
        {
            new GlucoseReading(105, _startTime),
            new GlucoseReading(118, _startTime.AddMinutes(5))
        };
    }

    private class FakeGlucoseReadingSource : IGlucoseReadingSource
    {
        private readonly IReadOnlyList<GlucoseReading> _readings;

        public FakeGlucoseReadingSource(IReadOnlyList<GlucoseReading> readings)
        {
            _readings = readings;
        }

        public IReadOnlyList<GlucoseReading> GetRecentReadings()
        {
            return _readings;
        }
    }

    private class FakeGlucoseReadingRepository : IGlucoseReadingRepository
    {
        public IReadOnlyList<GlucoseReading> SavedReadings { get; private set; } = Array.Empty<GlucoseReading>();

        public void SaveReadings(IReadOnlyList<GlucoseReading> readings)
        {
            SavedReadings = readings;
        }

        public IReadOnlyList<GlucoseReading> LoadReadings()
        {
            return SavedReadings;
        }
    }
}
