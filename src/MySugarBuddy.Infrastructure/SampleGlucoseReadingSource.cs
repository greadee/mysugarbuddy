using MySugarBuddy.Application;
using MySugarBuddy.Domain;

namespace MySugarBuddy.Infrastructure;

public class SampleGlucoseReadingSource : IGlucoseReadingSource
{
    public IReadOnlyList<GlucoseReading> GetRecentReadings()
    {
        var firstReadingTime = new DateTime(2026, 1, 15, 8, 0, 0);

        return new[]
        {
            new GlucoseReading(105, firstReadingTime),
            new GlucoseReading(118, firstReadingTime.AddMinutes(5))
        };
    }
}
