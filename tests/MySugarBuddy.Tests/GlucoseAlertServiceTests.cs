using MySugarBuddy.Application;
using MySugarBuddy.Domain;

namespace MySugarBuddy.Tests;

public class GlucoseAlertServiceTests
{
    private readonly DateTime _startTime = new(2026, 1, 15, 8, 0, 0);

    [Fact]
    public void FindsFastRisingAlert()
    {
        var service = new GlucoseAlertService();
        var previous = new GlucoseReading(100, _startTime);
        var current = new GlucoseReading(115, _startTime.AddMinutes(5));

        var alerts = service.CheckForAlerts(previous, current);

        Assert.Contains(GlucoseAlertType.RisingFast, alerts);
    }

    [Fact]
    public void ReturnsCalculatedTrend()
    {
        var service = new GlucoseAlertService();
        var previous = new GlucoseReading(100, _startTime);
        var current = new GlucoseReading(115, _startTime.AddMinutes(5));

        var result = service.CheckReadingPair(previous, current);

        Assert.Equal(GlucoseTrend.RisingFast, result.Trend);
    }

    [Fact]
    public void FindsLowAlertOnCurrentReading()
    {
        var service = new GlucoseAlertService();
        var previous = new GlucoseReading(90, _startTime);
        var current = new GlucoseReading(65, _startTime.AddMinutes(5));

        var alerts = service.CheckForAlerts(previous, current);

        Assert.Contains(GlucoseAlertType.Low, alerts);
    }

    [Fact]
    public void ReturnsNoAlertsForNormalStableReadings()
    {
        var service = new GlucoseAlertService();
        var previous = new GlucoseReading(105, _startTime);
        var current = new GlucoseReading(108, _startTime.AddMinutes(5));

        var alerts = service.CheckForAlerts(previous, current);

        Assert.Empty(alerts);
    }
}
