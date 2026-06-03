namespace MySugarBuddy.Domain;

public class GlucoseReading
{
    public GlucoseReading(int valueMgPerDl, DateTime recordedAt)
    {
        if (valueMgPerDl <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valueMgPerDl), "Glucose value must be positive.");
        }

        ValueMgPerDl = valueMgPerDl;
        RecordedAt = recordedAt;
    }

    public int ValueMgPerDl { get; }

    public DateTime RecordedAt { get; }

    public bool IsLow => ValueMgPerDl < 70;

    public bool IsHigh => ValueMgPerDl > 180;
}
