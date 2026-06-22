using MySugarBuddy.Domain;

namespace MySugarBuddy.Application;

public class GlucoseAlertResult
{
    public GlucoseAlertResult(GlucoseTrend trend, IReadOnlyList<GlucoseAlertType> alerts)
    {
        Trend = trend;
        Alerts = alerts;
    }

    public GlucoseTrend Trend { get; }

    public IReadOnlyList<GlucoseAlertType> Alerts { get; }
}
