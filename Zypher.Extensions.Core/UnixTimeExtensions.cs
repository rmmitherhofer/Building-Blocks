namespace Zypher.Extensions.Core;

public static class UnixTimeExtensions
{
    /// <summary>
    /// Converts a DateTime to Unix time expressed in seconds since 1970-01-01T00:00:00Z.
    /// The DateTimeKind is respected; local times are converted to UTC.
    /// </summary>
    /// <param name="date">The DateTime to convert.</param>
    /// <returns>Unix timestamp in seconds.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the date is before Unix epoch.</exception>
    public static long ToUnixTimeSeconds(this DateTime date)
    {
        var utcDate = date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();
        var dto = new DateTimeOffset(utcDate);

        if (dto < DateTimeOffset.UnixEpoch)
            throw new ArgumentOutOfRangeException(nameof(date), "Date cannot be before Unix epoch (1970-01-01T00:00:00Z)");

        return dto.ToUnixTimeSeconds();
    }

    /// <summary>
    /// Converts a Unix timestamp in seconds to a UTC DateTime.
    /// </summary>
    /// <param name="seconds">Unix timestamp in seconds. Must be non-negative.</param>
    /// <returns>Corresponding UTC DateTime.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if seconds is negative.</exception>
    public static DateTime FromUnixTimeSeconds(this long seconds)
    {
        if (seconds < 0)
            throw new ArgumentOutOfRangeException(nameof(seconds), "Unix time seconds cannot be negative.");

        return DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
    }

    /// <summary>
    /// Converts a DateTime to Unix time expressed in milliseconds since 1970-01-01T00:00:00Z.
    /// The DateTimeKind is respected; local times are converted to UTC.
    /// </summary>
    /// <param name="date">The DateTime to convert.</param>
    /// <returns>Unix timestamp in milliseconds.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the date is before Unix epoch.</exception>
    public static long ToUnixTimeMilliseconds(this DateTime date)
    {
        var utcDate = date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();
        var dto = new DateTimeOffset(utcDate);

        if (dto < DateTimeOffset.UnixEpoch)
            throw new ArgumentOutOfRangeException(nameof(date), "Date cannot be before Unix epoch (1970-01-01T00:00:00Z)");

        return dto.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Converts a Unix timestamp in milliseconds to a UTC DateTime.
    /// </summary>
    /// <param name="milliseconds">Unix timestamp in milliseconds. Must be non-negative.</param>
    /// <returns>Corresponding UTC DateTime.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if milliseconds is negative.</exception>
    public static DateTime FromUnixTimeMilliseconds(this long milliseconds)
    {
        if (milliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(milliseconds), "Unix time milliseconds cannot be negative.");

        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).UtcDateTime;
    }
}
