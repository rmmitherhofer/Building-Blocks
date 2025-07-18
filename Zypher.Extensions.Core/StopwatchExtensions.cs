using System.Diagnostics;

namespace Zypher.Extensions.Core;

/// <summary>
/// Extension methods for <see cref="Stopwatch"/> and time formatting utilities.
/// </summary>
public static class StopwatchExtensions
{
    /// <summary>
    /// Returns a formatted string of the stopwatch's elapsed time in the format "HH:mm:ss.fff".
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>A formatted time string.</returns>
    public static string GetFormattedTime(this Stopwatch stopwatch)
    {
        ArgumentNullException.ThrowIfNull(stopwatch);
        return stopwatch.Elapsed.ToString(@"hh\:mm\:ss\.fff");
    }

    /// <summary>
    /// Converts a long value representing milliseconds into a formatted time string "HH:mm:ss.fff".
    /// </summary>
    /// <param name="elapsedMilliseconds">Elapsed time in milliseconds.</param>
    /// <returns>A formatted time string.</returns>
    public static string GetFormattedTime(this long elapsedMilliseconds)
    {
        var time = TimeSpan.FromMilliseconds(elapsedMilliseconds);
        return time.ToString(@"hh\:mm\:ss\.fff");
    }

    /// <summary>
    /// Converts a double value representing milliseconds into a formatted time string "HH:mm:ss.fff".
    /// </summary>
    /// <param name="elapsedMilliseconds">Elapsed time in milliseconds.</param>
    /// <returns>A formatted time string.</returns>
    public static string GetFormattedTime(this double elapsedMilliseconds)
    {
        var time = TimeSpan.FromMilliseconds(elapsedMilliseconds);
        return time.ToString(@"hh\:mm\:ss\.fff");
    }

    /// <summary>
    /// Converts a time value and its unit into a formatted string "HH:mm:ss.fff".
    /// </summary>
    /// <param name="time">The numeric time value.</param>
    /// <param name="unit">The unit of time (milliseconds, seconds, etc.).</param>
    /// <returns>A formatted time string.</returns>
    public static string GetFormattedTime(this double time, TimeUnit unit = TimeUnit.Milliseconds)
    {
        TimeSpan ts = unit switch
        {
            TimeUnit.Milliseconds => TimeSpan.FromMilliseconds(time),
            TimeUnit.Seconds => TimeSpan.FromSeconds(time),
            TimeUnit.Minutes => TimeSpan.FromMinutes(time),
            TimeUnit.Hours => TimeSpan.FromHours(time),
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };

        return ts.ToString(@"hh\:mm\:ss\.fff");
    }

    /// <summary>
    /// Converts a numeric time value and unit into a <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="time">The numeric time value.</param>
    /// <param name="unit">The unit of time.</param>
    /// <returns>A <see cref="TimeSpan"/> representation of the time.</returns>
    public static TimeSpan ToTimeSpan(this double time, TimeUnit unit = TimeUnit.Milliseconds)
    {
        return unit switch
        {
            TimeUnit.Milliseconds => TimeSpan.FromMilliseconds(time),
            TimeUnit.Seconds => TimeSpan.FromSeconds(time),
            TimeUnit.Minutes => TimeSpan.FromMinutes(time),
            TimeUnit.Hours => TimeSpan.FromHours(time),
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };
    }

    /// <summary>
    /// Converts a time value and unit into a strongly-typed <see cref="ElapsedTimeInfo"/> structure.
    /// </summary>
    /// <param name="time">The numeric time value.</param>
    /// <param name="unit">The unit of time.</param>
    /// <returns>An <see cref="ElapsedTimeInfo"/> object with detailed breakdowns.</returns>
    public static ElapsedTimeInfo ToElapsedInfo(this double time, TimeUnit unit = TimeUnit.Milliseconds)
    {
        var span = time.ToTimeSpan(unit);
        return new ElapsedTimeInfo(
            ElapsedMilliseconds: (long)span.TotalMilliseconds,
            Formatted: span.ToString(@"hh\:mm\:ss\.fff"),
            TotalSeconds: span.TotalSeconds,
            TotalMinutes: span.TotalMinutes,
            TotalHours: span.TotalHours
        );
    }

    /// <summary>
    /// Returns a formatted time string "HH:mm:ss.fff" after rounding the time to a specified number of decimal places.
    /// </summary>
    /// <param name="time">The numeric time value.</param>
    /// <param name="unit">The unit of time.</param>
    /// <param name="decimals">Number of decimal places to round to.</param>
    /// <returns>A rounded and formatted time string.</returns>
    public static string GetRoundedFormattedTime(this double time, TimeUnit unit = TimeUnit.Milliseconds, int decimals = 0)
    {
        var ts = time.ToTimeSpan(unit);
        var rounded = TimeSpan.FromMilliseconds(Math.Round(ts.TotalMilliseconds, decimals));
        return rounded.ToString(@"hh\:mm\:ss\.fff");
    }

    /// <summary>
    /// Returns an <see cref="ElapsedTimeInfo"/> object that provides multiple views of the stopwatch's elapsed time.
    /// </summary>
    /// <param name="stopwatch">The stopwatch instance.</param>
    /// <returns>An <see cref="ElapsedTimeInfo"/> object.</returns>
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
    /// A strongly-typed container for various representations of elapsed time.
    /// </summary>
    /// <param name="ElapsedMilliseconds">The total elapsed time in milliseconds.</param>
    /// <param name="Formatted">The formatted elapsed time (e.g., "HH:mm:ss.fff").</param>
    /// <param name="TotalSeconds">The total elapsed time in seconds.</param>
    /// <param name="TotalMinutes">The total elapsed time in minutes.</param>
    /// <param name="TotalHours">The total elapsed time in hours.</param>
    public readonly record struct ElapsedTimeInfo(
        long ElapsedMilliseconds,
        string Formatted,
        double TotalSeconds,
        double TotalMinutes,
        double TotalHours
    );
}