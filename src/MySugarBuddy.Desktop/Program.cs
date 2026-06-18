using MySugarBuddy.Application;
using MySugarBuddy.Domain;
using MySugarBuddy.Infrastructure;

Console.WriteLine("My Sugar Buddy");
Console.WriteLine();

var readingSource = new SampleGlucoseReadingSource();
var alertService = new GlucoseAlertService();

var readings = readingSource.GetRecentReadings();
var previous = readings[0];
var current = readings[1];
var alerts = alertService.CheckForAlerts(previous, current);
var summary = GlucoseSummaryCalculator.Calculate(readings);

Console.WriteLine($"Previous reading: {previous.ValueMgPerDl} mg/dL at {previous.RecordedAt:t}");
Console.WriteLine($"Current reading:  {current.ValueMgPerDl} mg/dL at {current.RecordedAt:t}");
Console.WriteLine();
Console.WriteLine($"Readings loaded: {summary.ReadingCount}");
Console.WriteLine($"Average glucose: {summary.AverageValueMgPerDl:F1} mg/dL");
Console.WriteLine($"Range:           {summary.LowestValueMgPerDl}-{summary.HighestValueMgPerDl} mg/dL");
Console.WriteLine();

if (alerts.Count == 0)
{
    Console.WriteLine("No alerts for the sample readings.");
}
else
{
    Console.WriteLine("Sample alerts:");

    foreach (var alert in alerts)
    {
        Console.WriteLine($"- {alert}");
    }
}

Console.WriteLine();
Console.WriteLine("Dexcom sync, local storage, and real desktop notifications are not implemented yet.");
