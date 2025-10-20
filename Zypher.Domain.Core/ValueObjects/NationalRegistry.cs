namespace Zypher.Domain.Core.ValueObjects;

/// <summary>
/// Base class for Brazilian national registry numbers validation.
/// </summary>
public abstract class NationalRegistry
{
    /// <summary>
    /// Checks if the number is in the list of invalid repetitive sequences.
    /// </summary>
    /// <param name="number">Number string to validate.</param>
    /// <returns>True if invalid, otherwise false.</returns>
    protected static bool IsValid(string number)
    {
        var invalidNumbers = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            string value = string.Empty;
            for (int y = 0; y < number.Length; y++)
                value += i.ToString();

            invalidNumbers.Add(value);
        }
        return invalidNumbers.Contains(number);
    }

    /// <summary>
    /// Removes all non-digit characters from the input string.
    /// </summary>
    /// <param name="input">Input string.</param>
    /// <returns>Only digits from input.</returns>
    protected static string OnlyNumbers(string input)
        => new([.. input.Where(char.IsDigit)]);
}
