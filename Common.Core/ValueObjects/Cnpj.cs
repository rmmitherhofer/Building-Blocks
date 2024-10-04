using Common.Exceptions;

namespace Core.ValueObjects;

public abstract class CadastroNacional{
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

public class Cnpj : CadastroNacional
{
    public const int CnpjMaxLength = 14;
    public string Number { get; private set; }
    public string Inscricao { get; private set; }
    public string? Filial { get; private set; }
    public string Digit { get; private set; }

    public Cnpj(string number)
    {
        number = number.Replace(".", "").Replace("-", "").Replace("/", "");

        if (!Validate(number))
            throw new DomainException("CNPJ inválido.");

        Number = number.PadLeft(14, '0');

        Inscricao = Number[..8];
        Filial = Number.Substring(8, 4);
        Digit = Number.Substring(12, 2);
    }

    public static bool Validate(string cnpj)
    {
        int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int soma;
        int resto;
        string digito;
        string tempCnpj;
        cnpj = cnpj.Trim();
        cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

        cnpj = OnlyNumbers(cnpj);

        cnpj = cnpj.PadLeft(14, '0');

        if (IsValid(cnpj)) return false;

        tempCnpj = cnpj.Substring(0, 12);
        soma = 0;
        for (int i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;
        digito = resto.ToString();
        tempCnpj = tempCnpj + digito;
        soma = 0;
        for (int i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;
        digito = digito + resto.ToString();
        return cnpj.EndsWith(digito);
    }
}

public class Cpf : CadastroNacional
{
    public const int CpfMaxLength = 11;
    public string Number { get; private set; }
    public string Inscricao { get; private set; }
    public string Digit { get; private set; }

    public Cpf(string numero)
    {
        numero = numero.Replace(".", "").Replace("-", "");

        if (!Validar(numero))
            throw new DomainException("CPF inválido.");

        Number = numero.PadLeft(11, '0');

        Inscricao = Number[..9];
        Digit = Number.Substring(9, 2);
    }

    public static bool Validar(string cpf)
    {
        int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        string tempCpf;
        string digito;
        int soma;
        int resto;
        cpf = cpf.Trim();
        cpf = cpf.Replace(".", "").Replace("-", "");

        cpf = OnlyNumbers(cpf);

        if (IsValid(cpf)) return false;

        cpf = cpf.PadLeft(11, '0');
        tempCpf = cpf.Substring(0, 9);
        soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;
        digito = resto.ToString();
        tempCpf = tempCpf + digito;
        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
        resto = soma % 11;
        if (resto < 2)
            resto = 0;
        else
            resto = 11 - resto;
        digito = digito + resto.ToString();
        return cpf.EndsWith(digito);
    }
}
