using MySugarBuddy.Application;
using MySugarBuddy.Desktop;
using MySugarBuddy.Infrastructure;

Console.WriteLine("My Sugar Buddy");
Console.WriteLine();

var readingSource = new FileGlucoseReadingSource(Path.Combine("samples", "glucose-readings.csv"));
var readingRepository = new SqliteGlucoseReadingRepository(Path.Combine("data", "glucose-readings.db"));
var alertService = new GlucoseAlertService();
var notificationPort = new WindowsNotificationPort(new ConsoleNotificationPort());
var readingService = new GlucoseReadingService(readingSource, readingRepository, alertService, notificationPort);

var snapshot = readingService.RefreshReadings();
var previous = snapshot.PreviousReading;
var current = snapshot.CurrentReading;

if (previous is not null)
{
    Console.WriteLine($"Previous reading: {previous.ValueMgPerDl} mg/dL at {previous.RecordedAt:t}");
}

if (current is not null)
{
    Console.WriteLine($"Current reading:  {current.ValueMgPerDl} mg/dL at {current.RecordedAt:t}");
}

Console.WriteLine();
Console.WriteLine("Loaded sample readings from samples/glucose-readings.csv");
Console.WriteLine("Saved readings to data/glucose-readings.db");
Console.WriteLine($"Readings loaded: {snapshot.Summary.ReadingCount}");
Console.WriteLine($"Average glucose: {snapshot.Summary.AverageValueMgPerDl:F1} mg/dL");
Console.WriteLine($"Range:           {snapshot.Summary.LowestValueMgPerDl}-{snapshot.Summary.HighestValueMgPerDl} mg/dL");
Console.WriteLine($"Time in range:   {snapshot.Summary.InRangePercentage:F0}% ({snapshot.Summary.InRangeCount}/{snapshot.Summary.ReadingCount})");
Console.WriteLine($"GMI estimate:    {snapshot.Summary.GmiPercentage:F1}%");
if (snapshot.CurrentTrend is not null)
{
    Console.WriteLine($"Current trend:   {snapshot.CurrentTrend}");
}
Console.WriteLine();

Console.WriteLine("Recent readings:");
foreach (var reading in snapshot.RecentReadings(5))
{
    Console.WriteLine($"- {reading.RecordedAt:g}: {reading.ValueMgPerDl} mg/dL");
}
Console.WriteLine();

if (snapshot.Alerts.Count == 0)
{
    Console.WriteLine("No alerts for the sample readings.");
}
else
{
    Console.WriteLine("Sample alerts:");

    foreach (var alert in snapshot.Alerts)
    {
        Console.WriteLine($"- {alert}");
    }
}

Console.WriteLine();
Console.WriteLine("Dexcom sync is not implemented yet. Notifications use Windows when available, with console fallback.");
