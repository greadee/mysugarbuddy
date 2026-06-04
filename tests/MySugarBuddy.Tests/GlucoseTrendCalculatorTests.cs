using MySugarBuddy.Domain;

namespace MySugarBuddy.Tests;

public class GlucoseTrendCalculatorTests
{
    private readonly DateTime _startTime = new(2026, 1, 15, 8, 0, 0);

    [Fact]
    public void ReturnsStableForSmallChange()
    {
        var previous = new GlucoseReading(100, _startTime);
        var current = new GlucoseReading(103, _startTime.AddMinutes(5));

        var trend = GlucoseTrendCalculator.Calculate(previous, current);

        Assert.Equal(GlucoseTrend.Stable, trend);
    }

    [Fact]
    public void ReturnsRisingForModerateIncrease()
    {
        var previous = new GlucoseReading(100, _startTime);
        var current = new GlucoseReading(107, _startTime.AddMinutes(5));

        var trend = GlucoseTrendCalculator.Calculate(previous, current);

        Assert.Equal(GlucoseTrend.Rising, trend);
    }

    [Fact]
    public void ReturnsRisingFastForLargeIncrease()
    {
        var previous = new GlucoseReading(100, _startTime);
        var current = new GlucoseReading(115, _startTime.AddMinutes(5));

        var trend = GlucoseTrendCalculator.Calculate(previous, current);

        Assert.Equal(GlucoseTrend.RisingFast, trend);
    }

    [Fact]
    public void ReturnsDroppingForModerateDecrease()
    {
        var previous = new GlucoseReading(100, _startTime);
        var current = new GlucoseReading(93, _startTime.AddMinutes(5));

        var trend = GlucoseTrendCalculator.Calculate(previous, current);

        Assert.Equal(GlucoseTrend.Dropping, trend);
    }

    [Fact]
    public void ReturnsDroppingFastForLargeDecrease()
    {
        var previous = new GlucoseReading(100, _startTime);
        var current = new GlucoseReading(85, _startTime.AddMinutes(5));

        var trend = GlucoseTrendCalculator.Calculate(previous, current);

        Assert.Equal(GlucoseTrend.DroppingFast, trend);
    }

    [Fact]
    public void RejectsCurrentReadingWithOlderTimestamp()
    {
        var previous = new GlucoseReading(100, _startTime);
        var current = new GlucoseReading(110, _startTime.AddMinutes(-5));

        Assert.Throws<ArgumentException>(() =>
            GlucoseTrendCalculator.Calculate(previous, current));
    }
}
