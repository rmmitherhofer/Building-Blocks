using System.Globalization;

namespace Common.Extensions;

/// <summary>
/// Provides extension methods for parsing and validating date strings in Brazilian Portuguese format and other common formats.
/// </summary>
public static class DateTimeExtensions
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    private static readonly string[] SupportedFormats = new[]
    {
        "dd/MM/yyyy",
        "dd/MM/yyyy HH:mm:ss",
        "dd/MM/yyyy HH:mm",
        "dd/MM/yyyyTHH:mm:ss",
        "MM/yyyy",
        "yyyy-MM-dd",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss.fffZ"
    };

    /// <summary>
    /// Attempts to parse the input string to a nullable <see cref="DateTime"/> using supported formats.
    /// Returns null if parsing fails or input is null/empty.
    /// </summary>
    /// <param name="input">The date string to parse.</param>
    /// <returns>A nullable <see cref="DateTime"/> if parsing succeeds; otherwise, null.</returns>
    public static DateTime? ToDateTimeNullable(this string? input)
        => TryParseDate(input, out var result) ? result : null;

    /// <summary>
    /// Parses the input string to a <see cref="DateTime"/> using supported formats.
    /// Throws <see cref="FormatException"/> if parsing fails.
    /// </summary>
    /// <param name="input">The date string to parse.</param>
    /// <returns>The parsed <see cref="DateTime"/> value.</returns>
    /// <exception cref="FormatException">Thrown when the input string does not match any supported date format.</exception>
    public static DateTime ToDateTime(this string input)
    {
        if (TryParseDate(input, out var result) && result.HasValue)
            return result.Value;

        throw new FormatException($"Invalid date format. Supported formats: {string.Join(", ", SupportedFormats)}");
    }

    /// <summary>
    /// Tries to parse the input string to a nullable <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The date string to parse.</param>
    /// <param name="result">When this method returns, contains the parsed <see cref="DateTime"/> value if successful; otherwise, null.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public static bool TryParseDate(string? input, out DateTime? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (DateTime.TryParseExact(input, SupportedFormats, PtBr, DateTimeStyles.None, out var parsed))
        {
            result = parsed;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified string is a valid date according to supported formats.
    /// </summary>
    /// <param name="input">The date string to validate.</param>
    /// <returns>True if the input is a valid date; otherwise, false.</returns>
    public static bool IsValidDate(this string? input)
        => TryParseDate(input, out _);
}
