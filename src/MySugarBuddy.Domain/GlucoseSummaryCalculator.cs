namespace MySugarBuddy.Domain;

public static class GlucoseSummaryCalculator
{
    public static GlucoseSummary Calculate(IReadOnlyList<GlucoseReading> readings)
    {
        if (readings.Count == 0)
        {
            throw new ArgumentException("At least one reading is required.", nameof(readings));
        }

        var values = readings.Select(reading => reading.ValueMgPerDl).ToList();

        return new GlucoseSummary(
            readings.Count,
            values.Min(),
            values.Max(),
            values.Average());
    }
}
