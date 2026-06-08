using MySugarBuddy.Domain;

namespace MySugarBuddy.Tests;

public class GlucoseAlertEvaluatorTests
{
    [Fact]
    public void ReturnsLowAlertForLowReading()
    {
        var reading = new GlucoseReading(65, DateTime.Now);

        var alerts = GlucoseAlertEvaluator.GetAlerts(reading, GlucoseTrend.Stable);

        Assert.Contains(GlucoseAlertType.Low, alerts);
    }

    [Fact]
    public void ReturnsHighAlertForHighReading()
    {
        var reading = new GlucoseReading(190, DateTime.Now);

        var alerts = GlucoseAlertEvaluator.GetAlerts(reading, GlucoseTrend.Stable);

        Assert.Contains(GlucoseAlertType.High, alerts);
    }

    [Fact]
    public void ReturnsRisingFastAlertForFastRisingTrend()
    {
        var reading = new GlucoseReading(120, DateTime.Now);

        var alerts = GlucoseAlertEvaluator.GetAlerts(reading, GlucoseTrend.RisingFast);

        Assert.Contains(GlucoseAlertType.RisingFast, alerts);
    }

    [Fact]
    public void ReturnsDroppingFastAlertForFastDroppingTrend()
    {
        var reading = new GlucoseReading(120, DateTime.Now);

        var alerts = GlucoseAlertEvaluator.GetAlerts(reading, GlucoseTrend.DroppingFast);

        Assert.Contains(GlucoseAlertType.DroppingFast, alerts);
    }

    [Fact]
    public void ReturnsNoAlertsForNormalStableReading()
    {
        var reading = new GlucoseReading(110, DateTime.Now);

        var alerts = GlucoseAlertEvaluator.GetAlerts(reading, GlucoseTrend.Stable);

        Assert.Empty(alerts);
    }
}
