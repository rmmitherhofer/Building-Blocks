using System.Globalization;
using System.Text;

namespace Zypher.Extensions.Core;

public static class StringExtensions
{
    private static readonly CultureInfo Brazil = CultureInfo.GetCultureInfo("pt-BR");

    /// <summary>
    /// Applies the standard CPF or CNPJ mask to the input string.
    /// </summary>
    /// <param name="cpfCnpj">The raw CPF or CNPJ string.</param>
    /// <returns>The masked CPF or CNPJ string, or throws if input is invalid.</returns>
    /// <exception cref="ArgumentException">Thrown if input length is invalid.</exception>
    public static string CpfCnpjMask(this string cpfCnpj)
    {
        cpfCnpj = cpfCnpj.RemoveMaskCpfCnpj();

        if (cpfCnpj.Length <= 11)
        {
            cpfCnpj = cpfCnpj.PadLeft(11, '0');
            cpfCnpj = cpfCnpj.Insert(9, "-");
            cpfCnpj = cpfCnpj.Insert(6, ".");
            return cpfCnpj.Insert(3, ".");
        }
        else if (cpfCnpj.Length <= 14)
        {
            cpfCnpj = cpfCnpj.PadLeft(14, '0');
            cpfCnpj = cpfCnpj.Insert(12, "-");
            cpfCnpj = cpfCnpj.Insert(8, "/");
            cpfCnpj = cpfCnpj.Insert(5, ".");
            return cpfCnpj.Insert(2, ".");
        }

        throw new ArgumentException("Invalid CPF or CNPJ length.");
    }

    /// <summary>
    /// Applies the standard CPF mask to the input string.
    /// </summary>
    /// <param name="cpf">The raw CPF string.</param>
    /// <returns>The masked CPF string.</returns>
    /// <exception cref="ArgumentException">Thrown if input length is invalid.</exception>
    public static string CpfMask(this string cpf)
    {
        cpf = cpf.RemoveMaskCpfCnpj();

        if (cpf.Length <= 11)
        {
            cpf = cpf.PadLeft(11, '0');
            cpf = cpf.Insert(9, "-");
            cpf = cpf.Insert(6, ".");
            return cpf.Insert(3, ".");
        }

        throw new ArgumentException("Invalid CPF length.");
    }

    /// <summary>
    /// Applies the standard CNPJ mask to the input string.
    /// </summary>
    /// <param name="cnpj">The raw CNPJ string.</param>
    /// <returns>The masked CNPJ string.</returns>
    /// <exception cref="ArgumentException">Thrown if input length is invalid.</exception>
    public static string CnpjMask(this string cnpj)
    {
        cnpj = cnpj.RemoveMaskCpfCnpj();

        if (cnpj.Length > 11)
        {
            cnpj = cnpj.PadLeft(14, '0');
            cnpj = cnpj.Insert(12, "-");
            cnpj = cnpj.Insert(8, "/");
            cnpj = cnpj.Insert(5, ".");
            return cnpj.Insert(2, ".");
        }

        throw new ArgumentException("Invalid CNPJ length.");
    }

    /// <summary>
    /// Removes common masks (dots, dashes, slashes) from CPF or CNPJ strings.
    /// </summary>
    /// <param name="cpfCnpj">The masked CPF or CNPJ string.</param>
    /// <returns>The cleaned string with no mask characters.</returns>
    public static string RemoveMaskCpfCnpj(this string cpfCnpj)
    {
        return string.IsNullOrEmpty(cpfCnpj)
            ? string.Empty
            : new string(cpfCnpj.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Masks a registration number according to CPF format.
    /// </summary>
    /// <param name="registration">The registration string, expected length 7 or 8.</param>
    /// <returns>Formatted registration string.</returns>
    /// <exception cref="FormatException">If input is not numeric or length unexpected.</exception>
    public static string MaskRegistrationCpfCnpj(this string registration)
    {
        if (string.IsNullOrEmpty(registration))
            throw new ArgumentException("Registration cannot be null or empty.", nameof(registration));

        if (!long.TryParse(registration, out _))
            throw new FormatException("Registration must be numeric.");

        if (registration.Length == 8)
            return string.Format(@"{0:00\.000\.000}", double.Parse(registration));

        if (registration.Length == 7)
            return string.Format(@"{0:000\.000\.000}", double.Parse(registration));

        throw new FormatException("Registration length must be 7 or 8.");
    }

    /// <summary>
    /// Applies Brazilian phone number mask.
    /// </summary>
    /// <param name="telephone">Raw telephone number string.</param>
    /// <returns>Formatted telephone string or empty if invalid.</returns>
    public static string TelephoneMask(this string telephone)
    {
        if (string.IsNullOrEmpty(telephone)) return string.Empty;

        var digits = new string(telephone.Where(char.IsDigit).ToArray());

        if (digits.Length == 11)
        {
            return $"({digits.Substring(0, 2)}) {digits.Substring(2, 5)}-{digits.Substring(7, 4)}";
        }
        else if (digits.Length == 10)
        {
            return $"({digits.Substring(0, 2)}) {digits.Substring(2, 4)}-{digits.Substring(6, 4)}";
        }

        return string.Empty;
    }

    /// <summary>
    /// Applies Brazilian ZIP code mask.
    /// </summary>
    /// <param name="zipCode">Raw ZIP code string.</param>
    /// <returns>Formatted ZIP code or empty if invalid.</returns>
    public static string ZipCodeMask(this string zipCode)
    {
        if (string.IsNullOrEmpty(zipCode)) return string.Empty;

        var digits = new string(zipCode.Where(char.IsDigit).ToArray());

        if (digits.Length == 8)
        {
            return digits.Insert(5, "-");
        }

        return string.Empty;
    }

    /// <summary>
    /// Converts string to title case using pt-BR culture.
    /// </summary>
    /// <param name="text">Input text.</param>
    /// <returns>Text converted to title case.</returns>
    public static string ToTitleCase(this string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        var textInfo = new CultureInfo("pt-BR").TextInfo;
        return textInfo.ToTitleCase(text.ToLowerInvariant());
    }

    /// <summary>
    /// Returns empty string if input is null or empty; otherwise returns the input.
    /// </summary>
    /// <param name="text">Input text.</param>
    /// <returns>Empty string or original text.</returns>
    public static string EmptyIfNullOrEmpty(this string? text)
    {
        return string.IsNullOrEmpty(text) ? string.Empty : text;
    }

    /// <summary>
    /// Removes accents from characters in the string.
    /// </summary>
    /// <param name="text">Input text.</param>
    /// <returns>Text without diacritic marks.</returns>
    public static string ReplaceAccents(this string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Removes all spaces from the string.
    /// </summary>
    /// <param name="text">Input text.</param>
    /// <returns>Text without spaces.</returns>
    public static string RemoveSpaces(this string text)
    {
        return string.IsNullOrEmpty(text) ? string.Empty : text.Replace(" ", "");
    }
    /// <summary>
    /// Attempts to parse the input string as a Brazilian-style currency (e.g., “R$ 35.000,00”) and outputs the <see cref="decimal"/> value if successful.
    /// </summary>
    /// <param name="input">The string containing a Brazilian currency value (may include symbol “R$”, thousand separators, spaces).</param>
    /// <param name="value">When this method returns, contains the parsed decimal value if the parse succeeded; otherwise 0.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    public static bool TryParseBrazilianCurrency(this string? input, out decimal value)
    {
        value = 0m;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        string cleaned = input.Replace("R$", string.Empty, StringComparison.OrdinalIgnoreCase)
                              .Replace(".", string.Empty)
                              .Replace(" ", string.Empty)
                              .Trim();

        var styles = NumberStyles.AllowCurrencySymbol | NumberStyles.Number;

        return decimal.TryParse(cleaned, styles, Brazil, out value);
    }
    /// <summary>
    /// Parses the input string as a Brazilian-style currency (e.g., “R$ 35.000,00”) and returns the <see cref="decimal"/> value.
    /// </summary>
    /// <param name="input">The string containing a Brazilian currency value.</param>
    /// <returns>The <see cref="decimal"/> value represented by the string.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not in a valid Brazilian currency format.</exception>
    public static decimal? ParseBrazilianCurrency(this string? input)
    {
        if (input.TryParseBrazilianCurrency(out decimal value))
            return value;

        throw new FormatException($"String '{input}' não estava em um formato de moeda brasileiro válido.");
    }


    public static string ToBrazilianCurrencyString(this decimal? value)
    {
        if(!value.HasValue) return "R$ ";

        return value.Value.ToString("C", Brazil);
    }

    /// <summary>
    /// Determines whether the specified string is <c>null</c> or an empty string ("").
    /// </summary>
    /// <param name="input">The string to evaluate.</param>
    /// <returns><c>true</c> if the input is <c>null</c> or empty; otherwise, <c>false</c>.</returns>
    public static bool IsNullOrEmpty(this string? input) => string.IsNullOrEmpty(input);
    /// <summary>
    /// Determines whether the specified string is <c>null</c>, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="input">The string to evaluate.</param>
    /// <returns><c>true</c> if the input is <c>null</c>, empty, or consists exclusively of white-space characters; otherwise, <c>false</c>.</returns>
    public static bool IsNullOrWhiteSpace(this string? input) => string.IsNullOrWhiteSpace(input);

    /// <summary>
    /// Generates an alias from a full name by taking the uppercase initials of
    /// the first and last word. If only one word is present, returns only its initial.
    /// Returns an empty string if the input is null, whitespace or empty.
    /// </summary>
    public static string ToInitialsAlias(this string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return string.Empty;

        var parts = fullName
            .Trim()
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
            return string.Empty;

        char firstInitial = char.ToUpper(parts[0][0]);

        if (parts.Length == 1)
            return firstInitial.ToString();

        char lastInitial = char.ToUpper(parts[^1][0]);

        return $"{firstInitial}{lastInitial}";
    }
}
