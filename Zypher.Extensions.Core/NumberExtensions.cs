using System.Globalization;

namespace Zypher.Extensions.Core;

/// <summary>
/// Extension methods for formatting and parsing numbers with culture-specific settings.
/// </summary>
public static class NumberExtensions
{
    /// <summary>
    /// Formats a nullable decimal as a string in pt-BR culture format.
    /// Returns "0.00" if value is null.
    /// </summary>
    /// <param name="value">Nullable decimal value.</param>
    /// <returns>Formatted string in pt-BR format.</returns>
    public static string ToPtBr(this decimal? value)
    {
        if (value == null) return "0.00";

        return value.Value.FormatNumber(new CultureInfo("pt-BR"));
    }

    /// <summary>
    /// Formats a decimal as a string in pt-BR culture format.
    /// </summary>
    /// <param name="value">Decimal value.</param>
    /// <returns>Formatted string in pt-BR format.</returns>
    public static string ToPtBr(this decimal value)
        => value.FormatNumber(new CultureInfo("pt-BR"));

    /// <summary>
    /// Formats a decimal number with specified culture and number of decimal places.
    /// </summary>
    /// <param name="value">Decimal value.</param>
    /// <param name="culture">Culture info to format the number.</param>
    /// <param name="decimalPlaces">Number of decimal places (default is 2).</param>
    /// <returns>Formatted number string.</returns>
    public static string FormatNumber(this decimal value, CultureInfo culture, int decimalPlaces = 2)
    {
        if (culture == null) throw new ArgumentNullException(nameof(culture));

        return string.Format(culture, $"{{0:N{decimalPlaces}}}", value);
    }

    /// <summary>
    /// Parses a string representing a currency or number to decimal using optional culture.
    /// Throws <see cref="FormatException"/> if parsing fails.
    /// </summary>
    /// <param name="value">String value to parse.</param>
    /// <param name="culture">Optional culture info; if null, uses current culture.</param>
    /// <returns>Nullable decimal parsed from string.</returns>
    public static decimal? ParseToDecimal(this string? value, CultureInfo? culture = null)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        culture ??= CultureInfo.CurrentCulture;

        var normalized = NormalizeCurrencyString(value, culture);

        if (decimal.TryParse(normalized, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, culture, out var result))
            return result;

        throw new FormatException($"Failed to parse '{value}' to decimal using culture '{culture.Name}'.");
    }

    /// <summary>
    /// Normalizes a currency string by removing currency symbols, spaces, and formatting group and decimal separators.
    /// </summary>
    /// <param name="value">Currency string to normalize.</param>
    /// <param name="culture">Culture info used to identify separators.</param>
    /// <returns>Normalized string suitable for decimal parsing.</returns>
    private static string NormalizeCurrencyString(string value, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));

        string cleaned = value.RemoveCurrencySymbols().Replace(" ", "").Trim();

        var nfi = culture.NumberFormat;

        cleaned = cleaned.Replace(nfi.CurrencyGroupSeparator, "");
        cleaned = cleaned.Replace(nfi.CurrencyDecimalSeparator, ".");

        return cleaned;
    }
}
