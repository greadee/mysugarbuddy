using MySugarBuddy.Application;
using MySugarBuddy.Infrastructure;

Console.WriteLine("My Sugar Buddy");
Console.WriteLine();

var readingSource = new SampleGlucoseReadingSource();
var readingRepository = new JsonGlucoseReadingRepository(Path.Combine("data", "glucose-readings.json"));
var alertService = new GlucoseAlertService();
var readingService = new GlucoseReadingService(readingSource, readingRepository, alertService);

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
Console.WriteLine("Saved sample readings to data/glucose-readings.json");
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
Console.WriteLine("Dexcom sync, local storage, and real desktop notifications are not implemented yet.");
