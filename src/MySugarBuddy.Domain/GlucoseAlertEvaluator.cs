namespace MySugarBuddy.Domain;

public static class GlucoseAlertEvaluator
{
    public static IReadOnlyList<GlucoseAlertType> GetAlerts(GlucoseReading reading, GlucoseTrend trend)
    {
        var alerts = new List<GlucoseAlertType>();

        if (reading.IsLow)
        {
            alerts.Add(GlucoseAlertType.Low);
        }

        if (reading.IsHigh)
        {
            alerts.Add(GlucoseAlertType.High);
        }

        if (trend == GlucoseTrend.RisingFast)
        {
            alerts.Add(GlucoseAlertType.RisingFast);
        }

        if (trend == GlucoseTrend.DroppingFast)
        {
            alerts.Add(GlucoseAlertType.DroppingFast);
        }

        return alerts;
    }
}
