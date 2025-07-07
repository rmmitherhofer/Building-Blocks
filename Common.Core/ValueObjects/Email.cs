using Common.Exceptions;
using System.Text.RegularExpressions;

namespace Common.Core.ValueObjects;

/// <summary>
/// Represents an email address with validation.
/// </summary>
public partial class Email
{
    /// <summary>
    /// Maximum allowed length for an email address.
    /// </summary>
    public const int AddressMaxLength = 254;

    /// <summary>
    /// Minimum allowed length for an email address.
    /// </summary>
    public const int AddressMinLength = 5;

    /// <summary>
    /// The email address string.
    /// </summary>
    public string? Address { get; private set; }

    /// <summary>
    /// Creates a new Email instance after validating the provided address.
    /// </summary>
    /// <param name="address">Email address to validate and assign.</param>
    /// <exception cref="DomainException">Thrown when the email format is invalid.</exception>
    public Email(string? address)
    {
        if (address is null) return;

        if (!Validate(address)) throw new DomainException("Invalid email format.");

        Address = address;
    }

    /// <summary>
    /// Validates multiple email addresses separated by semicolon.
    /// Throws if any email is invalid.
    /// </summary>
    /// <param name="enderecos">String with email addresses separated by semicolons.</param>
    public static void IsValids(string? enderecos)
    {
        if (string.IsNullOrEmpty(enderecos)) return;

        foreach (var email in enderecos.Split([";"], StringSplitOptions.RemoveEmptyEntries))
            new Email(email);
    }

    /// <summary>
    /// Validates the email format using a regex.
    /// </summary>
    /// <param name="email">Email string to validate.</param>
    /// <returns>True if valid, otherwise false.</returns>
    public static bool Validate(string email)
        => EmailRegex().IsMatch(email);

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
