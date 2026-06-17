using MySugarBuddy.Infrastructure;

namespace MySugarBuddy.Tests;

public class SampleGlucoseReadingSourceTests
{
    [Fact]
    public void ReturnsTwoSampleReadings()
    {
        var source = new SampleGlucoseReadingSource();

        var readings = source.GetRecentReadings();

        Assert.Equal(2, readings.Count);
    }

    [Fact]
    public void ReturnsReadingsInTimeOrder()
    {
        var source = new SampleGlucoseReadingSource();

        var readings = source.GetRecentReadings();

        Assert.True(readings[0].RecordedAt < readings[1].RecordedAt);
    }

    [Fact]
    public void ReturnsExpectedSampleValues()
    {
        var source = new SampleGlucoseReadingSource();

        var readings = source.GetRecentReadings();

        Assert.Equal(105, readings[0].ValueMgPerDl);
        Assert.Equal(118, readings[1].ValueMgPerDl);
    }
}
