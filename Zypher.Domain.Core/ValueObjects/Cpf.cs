using Zypher.Domain.Exceptions;

namespace Zypher.Domain.Core.ValueObjects;

/// <summary>
/// Represents a Brazilian CPF (Cadastro de Pessoas Físicas).
/// </summary>
public class Cpf : NationalRegistry
{
    public const int CpfMaxLength = 11;

    /// <summary>
    /// The full CPF number as digits only.
    /// </summary>
    public string Number { get; private set; }

    /// <summary>
    /// The registration part of the CPF (first 9 digits).
    /// </summary>
    public string Registration { get; private set; }

    /// <summary>
    /// The verification digit (last 2 digits).
    /// </summary>
    public string Digit { get; private set; }

    /// <summary>
    /// Creates a new instance of <see cref="Cpf"/> after validation.
    /// </summary>
    /// <param name="number">CPF number string.</param>
    /// <exception cref="DomainException">Thrown when CPF is invalid.</exception>
    public Cpf(string number)
    {
        number = number.Replace(".", "").Replace("-", "");

        if (!Validar(number))
            throw new DomainException("Invalid CPF.");

        Number = number.PadLeft(11, '0');

        Registration = Number[..9];
        Digit = Number.Substring(9, 2);
    }

    /// <summary>
    /// Validates a given CPF number string.
    /// </summary>
    /// <param name="cpf">CPF string to validate.</param>
    /// <returns>True if valid, otherwise false.</returns>
    public static bool Validar(string cpf)
    {
        int[] multiplier1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplier2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];
        string tempCpf;
        string digit;
        int sum;
        int rest;
        cpf = cpf.Trim();
        cpf = cpf.Replace(".", "").Replace("-", "");

        cpf = OnlyNumbers(cpf);

        if (IsValid(cpf)) return false;

        cpf = cpf.PadLeft(11, '0');
        tempCpf = cpf[..9];
        sum = 0;

        for (int i = 0; i < 9; i++)
            sum += int.Parse(tempCpf[i].ToString()) * multiplier1[i];
        rest = sum % 11;
        if (rest < 2)
            rest = 0;
        else
            rest = 11 - rest;
        digit = rest.ToString();
        tempCpf += digit;
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(tempCpf[i].ToString()) * multiplier2[i];
        rest = sum % 11;
        if (rest < 2)
            rest = 0;
        else
            rest = 11 - rest;
        digit += rest.ToString();
        return cpf.EndsWith(digit);
    }
}
