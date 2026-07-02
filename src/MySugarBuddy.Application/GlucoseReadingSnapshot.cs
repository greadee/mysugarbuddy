using MySugarBuddy.Domain;

namespace MySugarBuddy.Application;

public class GlucoseReadingSnapshot
{
    public GlucoseReadingSnapshot(
        IReadOnlyList<GlucoseReading> readings,
        GlucoseSummary summary,
        GlucoseTrend? currentTrend,
        IReadOnlyList<GlucoseAlertType> alerts)
    {
        Readings = readings;
        Summary = summary;
        CurrentTrend = currentTrend;
        Alerts = alerts;
    }

    public IReadOnlyList<GlucoseReading> Readings { get; }

    public GlucoseSummary Summary { get; }

    public GlucoseTrend? CurrentTrend { get; }

    public IReadOnlyList<GlucoseAlertType> Alerts { get; }

    public GlucoseReading? PreviousReading => Readings.Count >= 2 ? Readings[^2] : null;

    public GlucoseReading? CurrentReading => Readings.Count >= 1 ? Readings[^1] : null;

    public IReadOnlyList<GlucoseReading> RecentReadings(int count)
    {
        if (count <= 0)
        {
            return Array.Empty<GlucoseReading>();
        }

        return Readings
            .Skip(Math.Max(0, Readings.Count - count))
            .ToList();
    }
}
