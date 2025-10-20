using System.Text.RegularExpressions;
using Zypher.Domain.Exceptions;

namespace Zypher.Domain.Core.ValueObjects;

/// <summary>
/// Represents a Brazilian CNPJ (Cadastro Nacional da Pessoa Jurídica),
/// the national registry number for legal entities. 
/// This class supports both numeric and alphanumeric CNPJs and provides
/// built-in validation and digit calculation using the official modulus 11 algorithm.
/// </summary>
public partial class Cnpj : NationalRegistry
{
    private static int BASE_CNPJ_LENGTH = 12;
    private static string REGEX_FORMATTING_CHARACTERS = "[./-]";
    private static Regex REGEX_BASE_PATTERN = BasePattern();
    private static Regex REGEX_DIGIT_PATTERN = DigitPattern();
    private static Regex REGEX_ZERO_VALUE = ZeroValue();
    private static int BASE_CHAR_VALUE = '0';
    private static int[] DIGIT_WEIGHTS = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];


    /// <summary>
    /// Gets the full CNPJ number as digits only (without formatting).
    /// </summary>
    public string Number { get; private set; }

    /// <summary>
    /// Gets the registration segment of the CNPJ (the first 8 digits).
    /// </summary>
    public string Registration { get; private set; }

    /// <summary>
    /// Gets the branch or subsidiary segment of the CNPJ (the next 4 digits).
    /// </summary>
    public string? Branch { get; private set; }

    /// <summary>
    /// Gets the verification digits of the CNPJ (the last 2 digits).
    /// </summary>
    public string Digit { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Cnpj"/> class after validating the provided number.
    /// </summary>
    /// <param name="number">The CNPJ number to validate and represent.</param>
    /// <exception cref="DomainException">Thrown when the CNPJ is invalid or improperly formatted.</exception>
    public Cnpj(string number)
    {
        number = number.Replace(".", "").Replace("-", "").Replace("/", "");

        if (!Validate(number))
            throw new DomainException("Invalid CNPJ.");

        Number = number.PadLeft(14, '0');
        Registration = Number[..8];
        Branch = Number.Substring(8, 4);
        Digit = Number.Substring(12, 2);
    }

    /// <summary>
    /// Validates a given CNPJ string, ensuring it follows structural and checksum rules.
    /// </summary>
    /// <param name="cnpj">The CNPJ string to validate (formatted or unformatted).</param>
    /// <returns><c>true</c> if the CNPJ is valid; otherwise, <c>false</c>.</returns>
    public static bool Validate(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj)) return false;

        cnpj = RemoveFormatting(cnpj);

        if (!IsCnpjFormatValidWithDigits(cnpj)) return false;

        string providedDigits = cnpj[BASE_CNPJ_LENGTH..];
        string calculatedDigits = CalculateVerificationDigits(cnpj[..BASE_CNPJ_LENGTH]);
        return calculatedDigits.Equals(providedDigits);
    }

    /// <summary>
    /// Calculates both verification digits for a given base CNPJ (first 12 characters).
    /// </summary>
    /// <param name="cnpj">The base CNPJ string (without verification digits).</param>
    /// <returns>The two calculated verification digits as a string.</returns>
    /// <exception cref="DomainException">Thrown if the CNPJ base is invalid for digit calculation.</exception>
    private static string CalculateVerificationDigits(string cnpj)
    {
        if (!string.IsNullOrWhiteSpace(cnpj))
        {
            cnpj = RemoveFormatting(cnpj);

            if (IsCnpjBaseFormatValid(cnpj))
            {
                string firstDigit = CalculateVerificationDigit(cnpj).ToString();
                string secondDigit = CalculateVerificationDigit(cnpj + firstDigit).ToString();
                return firstDigit + secondDigit;
            }
        }
        throw new DomainException("Invalid CNPJ for digit calculation.");
    }

    /// <summary>
    /// Calculates a single verification digit using the modulus 11 algorithm.
    /// </summary>
    /// <param name="cnpj">The CNPJ string to use in the calculation.</param>
    /// <returns>The calculated verification digit as an integer.</returns>
    private static int CalculateVerificationDigit(string cnpj)
    {
        int sum = 0;
        for (int i = cnpj.Length - 1; i >= 0; i--)
        {
            int number = cnpj[i] - BASE_CHAR_VALUE;
            sum += number * DIGIT_WEIGHTS[DIGIT_WEIGHTS.Length - cnpj.Length + i];
        }
        return sum % 11 < 2 ? 0 : 11 - (sum % 11);
    }

    /// <summary>
    /// Removes any formatting characters (., /, -) from a CNPJ string.
    /// </summary>
    /// <param name="cnpj">The CNPJ string to clean.</param>
    /// <returns>The CNPJ string containing only alphanumeric characters.</returns>
    public static string RemoveFormatting(string cnpj)
    {
        return Regex.Replace(cnpj.Trim(), REGEX_FORMATTING_CHARACTERS, "");
    }

    /// <summary>
    /// Determines whether the provided CNPJ base (without verification digits) has a valid format.
    /// </summary>
    /// <param name="cnpj">The CNPJ string to validate.</param>
    /// <returns><c>true</c> if the base format is valid; otherwise, <c>false</c>.</returns>
    private static bool IsCnpjBaseFormatValid(string cnpj)
    {
        return REGEX_BASE_PATTERN.IsMatch(cnpj) && !REGEX_ZERO_VALUE.IsMatch(cnpj);
    }

    /// <summary>
    /// Determines whether the provided CNPJ (including verification digits) has a valid format.
    /// </summary>
    /// <param name="cnpj">The full CNPJ string to validate.</param>
    /// <returns><c>true</c> if the format is valid; otherwise, <c>false</c>.</returns>
    private static bool IsCnpjFormatValidWithDigits(string cnpj)
    {
        var pattern = REGEX_BASE_PATTERN.ToString() + REGEX_DIGIT_PATTERN.ToString();
        var regex = new Regex($"^{pattern}");
        return regex.IsMatch(cnpj) && !REGEX_ZERO_VALUE.IsMatch(cnpj);
    }

    [GeneratedRegex(@"[A-Z\d]{12}")]
    private static partial Regex BasePattern();

    [GeneratedRegex(@"[\d]{2}")]
    private static partial Regex DigitPattern();

    [GeneratedRegex("^[0]+$")]
    private static partial Regex ZeroValue();
}
