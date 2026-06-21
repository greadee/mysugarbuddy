using MySugarBuddy.Domain;

namespace MySugarBuddy.Tests;

public class GlucoseSummaryCalculatorTests
{
    [Fact]
    public void CalculatesReadingCount()
    {
        var readings = CreateReadings();

        var summary = GlucoseSummaryCalculator.Calculate(readings);

        Assert.Equal(3, summary.ReadingCount);
    }

    [Fact]
    public void CalculatesLowestAndHighestValues()
    {
        var readings = CreateReadings();

        var summary = GlucoseSummaryCalculator.Calculate(readings);

        Assert.Equal(95, summary.LowestValueMgPerDl);
        Assert.Equal(125, summary.HighestValueMgPerDl);
    }

    [Fact]
    public void CalculatesAverageValue()
    {
        var readings = CreateReadings();

        var summary = GlucoseSummaryCalculator.Calculate(readings);

        Assert.Equal(110, summary.AverageValueMgPerDl);
    }

    [Fact]
    public void CalculatesInRangeCount()
    {
        var readings = CreateMixedRangeReadings();

        var summary = GlucoseSummaryCalculator.Calculate(readings);

        Assert.Equal(2, summary.InRangeCount);
    }

    [Fact]
    public void CalculatesInRangePercentage()
    {
        var readings = CreateMixedRangeReadings();

        var summary = GlucoseSummaryCalculator.Calculate(readings);

        Assert.Equal(50, summary.InRangePercentage);
    }

    [Fact]
    public void RejectsEmptyReadingList()
    {
        Assert.Throws<ArgumentException>(() =>
            GlucoseSummaryCalculator.Calculate(Array.Empty<GlucoseReading>()));
    }

    private static IReadOnlyList<GlucoseReading> CreateReadings()
    {
        var startTime = new DateTime(2026, 1, 15, 8, 0, 0);

        return new[]
        {
            new GlucoseReading(95, startTime),
            new GlucoseReading(110, startTime.AddMinutes(5)),
            new GlucoseReading(125, startTime.AddMinutes(10))
        };
    }

    private static IReadOnlyList<GlucoseReading> CreateMixedRangeReadings()
    {
        var startTime = new DateTime(2026, 1, 15, 8, 0, 0);

        return new[]
        {
            new GlucoseReading(65, startTime),
            new GlucoseReading(90, startTime.AddMinutes(5)),
            new GlucoseReading(150, startTime.AddMinutes(10)),
            new GlucoseReading(190, startTime.AddMinutes(15))
        };
    }
}
