using MySugarBuddy.Domain;

namespace MySugarBuddy.Application;

public interface IGlucoseReadingRepository
{
    void SaveReadings(IReadOnlyList<GlucoseReading> readings);

    IReadOnlyList<GlucoseReading> LoadReadings();
}
