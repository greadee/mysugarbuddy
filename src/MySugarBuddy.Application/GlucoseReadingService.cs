using MySugarBuddy.Domain;

namespace MySugarBuddy.Application;

public class GlucoseReadingService
{
    private readonly IGlucoseReadingSource _readingSource;
    private readonly IGlucoseReadingRepository _readingRepository;
    private readonly GlucoseAlertService _alertService;
    private readonly INotificationPort _notificationPort;

    public GlucoseReadingService(
        IGlucoseReadingSource readingSource,
        IGlucoseReadingRepository readingRepository,
        GlucoseAlertService alertService,
        INotificationPort? notificationPort = null)
    {
        _readingSource = readingSource;
        _readingRepository = readingRepository;
        _alertService = alertService;
        _notificationPort = notificationPort ?? NoOpNotificationPort.Instance;
    }

    public GlucoseReadingSnapshot RefreshReadings()
    {
        var recentReadings = _readingSource.GetRecentReadings();

        _readingRepository.SaveReadings(recentReadings);

        var savedReadings = _readingRepository.LoadReadings();
        var summary = GlucoseSummaryCalculator.Calculate(savedReadings);
        var alertResult = GetAlertResult(savedReadings);
        SendAlertNotification(alertResult, savedReadings);

        return new GlucoseReadingSnapshot(
            savedReadings,
            summary,
            alertResult?.Trend,
            alertResult?.Alerts ?? Array.Empty<GlucoseAlertType>());
    }

    private GlucoseAlertResult? GetAlertResult(IReadOnlyList<GlucoseReading> readings)
    {
        if (readings.Count < 2)
        {
            return null;
        }

        return _alertService.CheckReadingPair(readings[^2], readings[^1]);
    }

    private void SendAlertNotification(GlucoseAlertResult? alertResult, IReadOnlyList<GlucoseReading> readings)
    {
        if (alertResult is null || alertResult.Alerts.Count == 0 || readings.Count == 0)
        {
            return;
        }

        var current = readings[^1];
        var alertNames = string.Join(", ", alertResult.Alerts);
        var body = $"{current.ValueMgPerDl} mg/dL at {current.RecordedAt:t}. Trend: {alertResult.Trend}. Alerts: {alertNames}.";

        _notificationPort.Send(new NotificationMessage("Glucose alert", body));
    }
}
