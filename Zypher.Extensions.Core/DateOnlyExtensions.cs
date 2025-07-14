using System.Globalization;

namespace Zypher.Extensions.Core;

public static class DateOnlyExtensions
{
    /// <summary>
    /// Converts a DateTime to a DateOnly instance.
    /// </summary>
    /// <param name="dt">The DateTime to convert.</param>
    /// <returns>A DateOnly representing the date part of the DateTime.</returns>
    public static DateOnly ToDateOnly(this DateTime dt) => DateOnly.FromDateTime(dt);

    /// <summary>
    /// Tries to parse a string into a nullable DateOnly using the specified format and culture.
    /// Returns null if the input is null, empty or invalid.
    /// </summary>
    /// <param name="input">The input string to parse.</param>
    /// <param name="format">The expected date format. Default is "dd/MM/yyyy".</param>
    /// <param name="culture">The culture name for parsing. Default is "pt-BR".</param>
    /// <returns>A nullable DateOnly parsed from the input, or null if parsing fails.</returns>
    public static DateOnly? ToDateOnly(this string? input, string format = "dd/MM/yyyy", string culture = "pt-BR")
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        var provider = new CultureInfo(culture);

        if (DateOnly.TryParseExact(input, format, provider, DateTimeStyles.None, out var date))
            return date;

        return null;
    }

    /// <summary>
    /// Formats a DateOnly value as a string in the "dd/MM/yyyy" format for pt-BR culture.
    /// </summary>
    /// <param name="date">The DateOnly value.</param>
    /// <returns>The formatted date string.</returns>
    public static string ToPtBr(this DateOnly date) => date.ToString("dd/MM/yyyy", new CultureInfo("pt-BR"));
}
