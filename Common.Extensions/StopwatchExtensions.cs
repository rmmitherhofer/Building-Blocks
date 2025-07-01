using System.Diagnostics;

namespace Common.Extensions;

public static class StopwatchExtensions
{
    public static string GetTime(this Stopwatch stopwatch)
    {
        if (stopwatch is null)
            ArgumentNullException.ThrowIfNull(stopwatch, nameof(Stopwatch));
       
        return GetTime(stopwatch.ElapsedMilliseconds);
    }

    public static string GetTime(this long elapsedMilliseconds)
    {
        var time = TimeSpan.FromMilliseconds(elapsedMilliseconds);
        var timeOnly = TimeOnly.FromTimeSpan(time);

        return timeOnly.ToString("HH:mm:ss.fff");
    }
}
