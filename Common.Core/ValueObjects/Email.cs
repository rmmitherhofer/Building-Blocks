using Common.Exceptions;
using System.Text.RegularExpressions;

namespace Core.ValueObjects;

public class Email
{
    public const int AddressMaxLength = 254;
    public const int AddressMinLength = 5;

    public string? Address {  get; private set; }

    public Email(string? address)
    {
        if (address is null) return;

        if (!Validate(address)) throw new DomainException($"e-mail {address} inválido");

        Address = address;
    }

    public static void IsValids(string? enderecos)
    {
        if (string.IsNullOrEmpty(enderecos)) return;

        foreach (var email in enderecos.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))        
            new Email(email);        
    }

    public static bool Validate(string email) 
        => new Regex("^(?(\")(\".+?\"@)|(([0-9a-zA-Z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-zA-Z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,6}))$").IsMatch(email);
}
