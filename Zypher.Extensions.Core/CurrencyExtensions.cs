using System.Globalization;

namespace Zypher.Extensions.Core;

/// <summary>
/// Provides extension methods to format decimals as currency strings and parse currency strings to decimals,
/// supporting multiple cultures with fallback and symbol removal.
/// </summary>
public static class CurrencyExtensions
{
    private static readonly CultureInfo PtBr = new("pt-BR");
    private static readonly CultureInfo EnUs = new("en-US");
    private static readonly CultureInfo EnGb = new("en-GB");

    private static readonly CultureInfo[] DefaultCultures = new[] { PtBr, EnUs, EnGb };

    /// <summary>
    /// Converts a decimal value to a currency-formatted string using the specified culture.
    /// </summary>
    /// <param name="value">The decimal value to format.</param>
    /// <param name="culture">The culture to use for formatting.</param>
    /// <returns>A string representing the currency formatted value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="culture"/> is null.</exception>
    public static string ToCurrency(this decimal value, CultureInfo culture)
        => string.Format(culture ?? throw new ArgumentNullException(nameof(culture)), "{0:C}", value);

    /// <summary>
    /// Converts a decimal value to a currency-formatted string using the specified culture code.
    /// </summary>
    /// <param name="value">The decimal value to format.</param>
    /// <param name="cultureCode">The culture code (e.g. "pt-BR", "en-US").</param>
    /// <returns>A string representing the currency formatted value.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="cultureCode"/> is null or empty.</exception>
    public static string ToCurrency(this decimal value, string cultureCode)
    {
        if (string.IsNullOrEmpty(cultureCode))
            throw new ArgumentException("Culture code must be provided.", nameof(cultureCode));
        return value.ToCurrency(new CultureInfo(cultureCode));
    }

    /// <summary>
    /// Converts a nullable decimal value to a currency-formatted string using the specified culture.
    /// Returns zero currency if the value is null.
    /// </summary>
    /// <param name="value">The nullable decimal value to format.</param>
    /// <param name="culture">The culture to use for formatting.</param>
    /// <returns>A string representing the currency formatted value or zero currency if null.</returns>
    public static string ToCurrency(this decimal? value, CultureInfo culture)
        => value.HasValue ? value.Value.ToCurrency(culture) : GetCurrencySymbol(culture) + "0.00";

    /// <summary>
    /// Converts a nullable decimal value to a currency-formatted string using the specified culture code.
    /// Returns zero currency if the value is null.
    /// </summary>
    /// <param name="value">The nullable decimal value to format.</param>
    /// <param name="cultureCode">The culture code (e.g. "pt-BR", "en-US").</param>
    /// <returns>A string representing the currency formatted value or zero currency if null.</returns>
    public static string ToCurrency(this decimal? value, string cultureCode)
        => value.ToCurrency(new CultureInfo(cultureCode));

    /// <summary>
    /// Parses a currency string into a decimal value, attempting with the specified culture and fallback cultures.
    /// Currency symbols are removed before parsing.
    /// </summary>
    /// <param name="input">The currency string to parse.</param>
    /// <param name="culture">The culture to use for parsing. If null, fallback cultures are used.</param>
    /// <returns>The parsed decimal value.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="input"/> is null or empty.</exception>
    /// <exception cref="FormatException">Thrown if parsing fails.</exception>
    public static decimal ToDecimal(this string input, CultureInfo? culture = null)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input is null or empty.", nameof(input));

        string cleaned = input.RemoveCurrencySymbols();

        if (culture != null && decimal.TryParse(cleaned, NumberStyles.Currency, culture, out var result))
            return result;

        foreach (var fallback in DefaultCultures)
        {
            if (decimal.TryParse(cleaned, NumberStyles.Currency, fallback, out result))
                return result;
        }

        throw new FormatException($"Unable to parse '{input}' as a valid currency.");
    }

    /// <summary>
    /// Tries to parse a currency string into a decimal value.
    /// </summary>
    /// <param name="input">The currency string to parse.</param>
    /// <param name="result">The parsed decimal value if successful.</param>
    /// <param name="culture">The culture to use for parsing. If null, fallback cultures are used.</param>
    /// <returns>True if parsing succeeded, false otherwise.</returns>
    public static bool TryParseCurrency(this string input, out decimal result, CultureInfo? culture = null)
    {
        result = 0;
        if (string.IsNullOrWhiteSpace(input)) return false;

        string cleaned = input.RemoveCurrencySymbols();

        if (culture != null)
            return decimal.TryParse(cleaned, NumberStyles.Currency, culture, out result);

        foreach (var fallback in DefaultCultures)
        {
            if (decimal.TryParse(cleaned, NumberStyles.Currency, fallback, out result))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Removes common currency symbols from the string.
    /// </summary>
    /// <param name="value">The string to clean.</param>
    /// <returns>The string without currency symbols or null if input is null/empty.</returns>
    public static string? RemoveCurrencySymbols(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        string[] symbols = new[] { "R$", "$", "£", "€" };
        foreach (var symbol in symbols)
            value = value.Replace(symbol, "", StringComparison.OrdinalIgnoreCase);

        return value.Trim();
    }

    /// <summary>
    /// Gets the currency symbol for the specified culture.
    /// </summary>
    /// <param name="culture">The culture to get the symbol for.</param>
    /// <returns>The currency symbol string or empty if culture is null.</returns>
    public static string GetCurrencySymbol(CultureInfo culture)
        => culture?.NumberFormat.CurrencySymbol ?? string.Empty;
}
