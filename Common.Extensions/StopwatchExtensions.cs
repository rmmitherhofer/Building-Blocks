using System.Diagnostics;

namespace Common.Extensions;

/// <summary>
/// Extension methods for Stopwatch and elapsed time formatting.
/// </summary>
public static class StopwatchExtensions
{
    /// <summary>
    /// Returns the elapsed time of the Stopwatch formatted as "HH:mm:ss.fff".
    /// </summary>
    /// <param name="stopwatch">The Stopwatch instance.</param>
    /// <returns>Formatted elapsed time string.</returns>
    public static string GetTime(this Stopwatch stopwatch)
    {
        ArgumentNullException.ThrowIfNull(stopwatch, nameof(stopwatch));
        return GetTime(stopwatch.ElapsedMilliseconds);
    }

    /// <summary>
    /// Converts elapsed milliseconds to a formatted string "HH:mm:ss.fff".
    /// </summary>
    /// <param name="elapsedMilliseconds">Elapsed time in milliseconds.</param>
    /// <returns>Formatted time string.</returns>
    public static string GetTime(this long elapsedMilliseconds)
    {
        var time = TimeSpan.FromMilliseconds(elapsedMilliseconds);

        var timeOnly = TimeOnly.FromTimeSpan(time);
        return timeOnly.ToString("HH:mm:ss.fff");
    }
}
