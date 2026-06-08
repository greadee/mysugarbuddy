using MySugarBuddy.Domain;

namespace MySugarBuddy.Application;

public class GlucoseAlertService
{
    public IReadOnlyList<GlucoseAlertType> CheckForAlerts(GlucoseReading previous, GlucoseReading current)
    {
        var trend = GlucoseTrendCalculator.Calculate(previous, current);

        return GlucoseAlertEvaluator.GetAlerts(current, trend);
    }
}
