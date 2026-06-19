using MySugarBuddy.Domain;

namespace MySugarBuddy.Application;

public class GlucoseReadingService
{
    private readonly IGlucoseReadingSource _readingSource;
    private readonly IGlucoseReadingRepository _readingRepository;
    private readonly GlucoseAlertService _alertService;

    public GlucoseReadingService(
        IGlucoseReadingSource readingSource,
        IGlucoseReadingRepository readingRepository,
        GlucoseAlertService alertService)
    {
        _readingSource = readingSource;
        _readingRepository = readingRepository;
        _alertService = alertService;
    }

    public GlucoseReadingSnapshot RefreshReadings()
    {
        var recentReadings = _readingSource.GetRecentReadings();

        _readingRepository.SaveReadings(recentReadings);

        var savedReadings = _readingRepository.LoadReadings();
        var summary = GlucoseSummaryCalculator.Calculate(savedReadings);
        var alerts = GetAlerts(savedReadings);

        return new GlucoseReadingSnapshot(savedReadings, summary, alerts);
    }

    private IReadOnlyList<GlucoseAlertType> GetAlerts(IReadOnlyList<GlucoseReading> readings)
    {
        if (readings.Count < 2)
        {
            return Array.Empty<GlucoseAlertType>();
        }

        return _alertService.CheckForAlerts(readings[^2], readings[^1]);
    }
}
