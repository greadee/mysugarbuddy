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
        var inRangeCount = readings.Count(reading => !reading.IsLow && !reading.IsHigh);

        var averageValue = values.Average();

        return new GlucoseSummary(
            readings.Count,
            values.Min(),
            values.Max(),
            averageValue,
            inRangeCount,
            (double)inRangeCount / readings.Count * 100,
            CalculateGmiPercentage(averageValue));
    }

    private static double CalculateGmiPercentage(double averageValueMgPerDl)
    {
        return 3.31 + 0.02392 * averageValueMgPerDl;
    }
}
