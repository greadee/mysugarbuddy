using MySugarBuddy.Domain;

namespace MySugarBuddy.Application;

public interface IGlucoseReadingSource
{
    IReadOnlyList<GlucoseReading> GetRecentReadings();
}
