namespace MySugarBuddy.Domain;

public class GlucoseSummary
{
    public GlucoseSummary(int readingCount, int lowestValueMgPerDl, int highestValueMgPerDl, double averageValueMgPerDl)
    {
        ReadingCount = readingCount;
        LowestValueMgPerDl = lowestValueMgPerDl;
        HighestValueMgPerDl = highestValueMgPerDl;
        AverageValueMgPerDl = averageValueMgPerDl;
    }

    public int ReadingCount { get; }

    public int LowestValueMgPerDl { get; }

    public int HighestValueMgPerDl { get; }

    public double AverageValueMgPerDl { get; }
}
