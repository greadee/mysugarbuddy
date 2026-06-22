using MySugarBuddy.Domain;

namespace MySugarBuddy.Application;

public class GlucoseAlertService
{
    public IReadOnlyList<GlucoseAlertType> CheckForAlerts(GlucoseReading previous, GlucoseReading current)
    {
        return CheckReadingPair(previous, current).Alerts;
    }

    public GlucoseAlertResult CheckReadingPair(GlucoseReading previous, GlucoseReading current)
    {
        var trend = GlucoseTrendCalculator.Calculate(previous, current);
        var alerts = GlucoseAlertEvaluator.GetAlerts(current, trend);

        return new GlucoseAlertResult(trend, alerts);
    }
}
