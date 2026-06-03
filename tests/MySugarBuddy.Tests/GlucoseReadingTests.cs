using MySugarBuddy.Domain;

namespace MySugarBuddy.Tests;

public class GlucoseReadingTests
{
    [Fact]
    public void StoresValueAndTime()
    {
        var recordedAt = new DateTime(2026, 1, 15, 8, 30, 0);

        var reading = new GlucoseReading(110, recordedAt);

        Assert.Equal(110, reading.ValueMgPerDl);
        Assert.Equal(recordedAt, reading.RecordedAt);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void RejectsInvalidValues(int valueMgPerDl)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new GlucoseReading(valueMgPerDl, DateTime.Now));
    }

    [Fact]
    public void CanTellWhenReadingIsLow()
    {
        var reading = new GlucoseReading(65, DateTime.Now);

        Assert.True(reading.IsLow);
        Assert.False(reading.IsHigh);
    }

    [Fact]
    public void CanTellWhenReadingIsHigh()
    {
        var reading = new GlucoseReading(190, DateTime.Now);

        Assert.True(reading.IsHigh);
        Assert.False(reading.IsLow);
    }
}
