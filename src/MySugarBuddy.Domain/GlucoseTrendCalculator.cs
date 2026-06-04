namespace MySugarBuddy.Domain;

public static class GlucoseTrendCalculator
{
    public static GlucoseTrend Calculate(GlucoseReading previous, GlucoseReading current)
    {
        var minutesBetweenReadings = (current.RecordedAt - previous.RecordedAt).TotalMinutes;

        if (minutesBetweenReadings <= 0)
        {
            throw new ArgumentException("Current reading must be newer than previous reading.");
        }

        var changePerMinute = (current.ValueMgPerDl - previous.ValueMgPerDl) / minutesBetweenReadings;

        if (changePerMinute >= 2)
        {
            return GlucoseTrend.RisingFast;
        }

        if (changePerMinute >= 1)
        {
            return GlucoseTrend.Rising;
        }

        if (changePerMinute <= -2)
        {
            return GlucoseTrend.DroppingFast;
        }

        if (changePerMinute <= -1)
        {
            return GlucoseTrend.Dropping;
        }

        return GlucoseTrend.Stable;
    }
}
