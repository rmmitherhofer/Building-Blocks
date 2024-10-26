namespace Common.Extensions;

public static class StopwatchExtensions
{
    public static string GetTime(this long elapsedMilliseconds)
    {
        var time = TimeSpan.FromMilliseconds(elapsedMilliseconds);
        var timeOnly = TimeOnly.FromTimeSpan(time);

        return timeOnly.ToString("HH:mm:ss.fff");
    }
}
