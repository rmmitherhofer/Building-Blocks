using System.Globalization;

namespace Common.Extensions;

public static class TimeOnlyExtensions
{
    /// <summary>
    /// Converts a <see cref="DateTime"/> to <see cref="TimeOnly"/>.
    /// </summary>
    /// <param name="dt">The DateTime instance.</param>
    /// <returns>The time component as TimeOnly.</returns>
    public static TimeOnly ToTimeOnly(this DateTime dt) => TimeOnly.FromDateTime(dt);

    /// <summary>
    /// Parses a string to <see cref="TimeOnly"/> using the specified format and culture.
    /// Returns null if parsing fails or input is null/empty.
    /// </summary>
    /// <param name="input">The time string to parse.</param>
    /// <param name="format">The expected time format. Default is "HH:mm:ss".</param>
    /// <param name="culture">The culture name for parsing. Default is "pt-BR".</param>
    /// <returns>A nullable TimeOnly parsed from the input, or null if invalid.</returns>
    public static TimeOnly? ToTimeOnly(this string? input, string format = "HH:mm:ss", string culture = "pt-BR")
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var provider = new CultureInfo(culture);

        if (TimeOnly.TryParseExact(input, format, provider, DateTimeStyles.None, out var time))
            return time;

        return null;
    }

    /// <summary>
    /// Formats the <see cref="TimeOnly"/> as a 24-hour string "HH:mm".
    /// </summary>
    /// <param name="time">The TimeOnly instance.</param>
    /// <returns>Formatted time string.</returns>
    public static string To24H(this TimeOnly time) => time.ToString("HH:mm");
}
