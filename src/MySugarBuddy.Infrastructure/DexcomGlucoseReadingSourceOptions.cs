namespace MySugarBuddy.Infrastructure;

public class DexcomGlucoseReadingSourceOptions
{
    public string AccessToken { get; init; } = string.Empty;

    public Uri BaseUri { get; init; } = new("https://api.dexcom.com");

    public TimeSpan Lookback { get; init; } = TimeSpan.FromHours(3);

    public Func<DateTime> UtcNow { get; init; } = () => DateTime.UtcNow;
}
