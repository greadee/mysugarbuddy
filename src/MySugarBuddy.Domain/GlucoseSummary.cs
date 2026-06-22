namespace MySugarBuddy.Domain;

public class GlucoseSummary
{
    public GlucoseSummary(
        int readingCount,
        int lowestValueMgPerDl,
        int highestValueMgPerDl,
        double averageValueMgPerDl,
        int inRangeCount,
        double inRangePercentage,
        double gmiPercentage)
    {
        ReadingCount = readingCount;
        LowestValueMgPerDl = lowestValueMgPerDl;
        HighestValueMgPerDl = highestValueMgPerDl;
        AverageValueMgPerDl = averageValueMgPerDl;
        InRangeCount = inRangeCount;
        InRangePercentage = inRangePercentage;
        GmiPercentage = gmiPercentage;
    }

    public int ReadingCount { get; }

    public int LowestValueMgPerDl { get; }

    public int HighestValueMgPerDl { get; }

    public double AverageValueMgPerDl { get; }

    public int InRangeCount { get; }

    public double InRangePercentage { get; }

    public double GmiPercentage { get; }
}
