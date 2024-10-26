using Common.Exceptions;

namespace Core.ValueObjects;

public abstract class NationalRegistry{
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

    protected static string OnlyNumbers(string input)
        => new string(input.Where(char.IsDigit).ToArray());
}

public class Cnpj : NationalRegistry
{
    public const int CnpjMaxLength = 14;
    public string Number { get; private set; }
    public string Registration { get; private set; }
    public string? Branch { get; private set; }
    public string Digit { get; private set; }

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

    public static bool Validate(string cnpj)
    {
        int[] multiplier1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplier2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int sum;
        int rest;
        string digit;
        string tempCnpj;
        cnpj = cnpj.Trim();
        cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

        cnpj = OnlyNumbers(cnpj);

        cnpj = cnpj.PadLeft(14, '0');

        if (IsValid(cnpj)) return false;

        tempCnpj = cnpj[..12];
        sum = 0;
        for (int i = 0; i < 12; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * multiplier1[i];
        rest = sum % 11;
        if (rest < 2)
            rest = 0;
        else
            rest = 11 - rest;
        digit = rest.ToString();
        tempCnpj += digit;
        sum = 0;
        for (int i = 0; i < 13; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * multiplier2[i];
        rest = sum % 11;
        if (rest < 2)
            rest = 0;
        else
            rest = 11 - rest;
        digit += rest.ToString();
        return cnpj.EndsWith(digit);
    }
}

public class Cpf : NationalRegistry
{
    public const int CpfMaxLength = 11;
    public string Number { get; private set; }
    public string Registration { get; private set; }
    public string Digit { get; private set; }

    public Cpf(string number)
    {
        number = number.Replace(".", "").Replace("-", "");

        if (!Validar(number))
            throw new DomainException("Invalid CPF.");

        Number = number.PadLeft(11, '0');

        Registration = Number[..9];
        Digit = Number.Substring(9, 2);
    }

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
