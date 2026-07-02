using MySugarBuddy.Application;
using MySugarBuddy.Domain;

namespace MySugarBuddy.Tests;

public class GlucoseReadingSnapshotTests
{
    [Fact]
    public void RecentReadingsReturnsLastReadingsInSavedOrder()
    {
        var readings = CreateReadings();
        var snapshot = CreateSnapshot(readings);

        var recentReadings = snapshot.RecentReadings(2);

        Assert.Equal(2, recentReadings.Count);
        Assert.Equal(readings[1].RecordedAt, recentReadings[0].RecordedAt);
        Assert.Equal(readings[2].RecordedAt, recentReadings[1].RecordedAt);
    }

    [Fact]
    public void RecentReadingsReturnsAllReadingsWhenCountIsTooHigh()
    {
        var readings = CreateReadings();
        var snapshot = CreateSnapshot(readings);

        var recentReadings = snapshot.RecentReadings(10);

        Assert.Equal(readings.Count, recentReadings.Count);
        Assert.Equal(readings[0].RecordedAt, recentReadings[0].RecordedAt);
        Assert.Equal(readings[2].RecordedAt, recentReadings[2].RecordedAt);
    }

    [Fact]
    public void RecentReadingsReturnsEmptyListWhenCountIsZero()
    {
        var snapshot = CreateSnapshot(CreateReadings());

        var recentReadings = snapshot.RecentReadings(0);

        Assert.Empty(recentReadings);
    }

    private static GlucoseReadingSnapshot CreateSnapshot(IReadOnlyList<GlucoseReading> readings)
    {
        var summary = GlucoseSummaryCalculator.Calculate(readings);

        return new GlucoseReadingSnapshot(
            readings,
            summary,
            GlucoseTrend.Stable,
            Array.Empty<GlucoseAlertType>());
    }

    private static IReadOnlyList<GlucoseReading> CreateReadings()
    {
        var startTime = new DateTime(2026, 1, 15, 8, 0, 0);

        return new[]
        {
            new GlucoseReading(100, startTime),
            new GlucoseReading(115, startTime.AddMinutes(5)),
            new GlucoseReading(120, startTime.AddMinutes(10))
        };
    }
}
