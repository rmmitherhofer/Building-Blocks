using System.Diagnostics;

namespace Zypher.Extensions.Core;

/// <summary>
/// Extension methods for Stopwatch and elapsed time formatting.
/// </summary>
public static class StopwatchExtensions
{
    /// <summary>
    /// Returns a formatted string of the elapsed time: "HH:mm:ss.fff".
    /// </summary>
    public static string GetFormattedTime(this Stopwatch stopwatch)
    {
        ArgumentNullException.ThrowIfNull(stopwatch);
        return stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
    }

    /// <summary>
    /// Converts milliseconds into a formatted time string "HH:mm:ss.fff".
    /// </summary>
    public static string GetFormattedTime(this long elapsedMilliseconds)
    {
        var time = TimeSpan.FromMilliseconds(elapsedMilliseconds);
        return time.ToString(@"hh\:mm\:ss\.fff");
    }

    /// <summary>
    /// Returns a dictionary-like object with multiple views of the stopwatch’s elapsed time.
    /// Useful for logging or diagnostics.
    /// </summary>
    public static ElapsedTimeInfo GetElapsedInfo(this Stopwatch stopwatch)
    {
        ArgumentNullException.ThrowIfNull(stopwatch);
        var elapsed = stopwatch.Elapsed;

        return new ElapsedTimeInfo
        (
            ElapsedMilliseconds: stopwatch.ElapsedMilliseconds,
            Formatted: elapsed.ToString(@"hh\:mm\:ss\.fff"),
            TotalSeconds: elapsed.TotalSeconds,
            TotalMinutes: elapsed.TotalMinutes,
            TotalHours: elapsed.TotalHours
        );
    }

    /// <summary>
    /// Strongly-typed container for various representations of stopwatch elapsed time.
    /// </summary>
    public readonly record struct ElapsedTimeInfo(
        long ElapsedMilliseconds,
        string Formatted,
        double TotalSeconds,
        double TotalMinutes,
        double TotalHours
    );
}