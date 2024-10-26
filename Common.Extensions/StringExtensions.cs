using System.Globalization;
using System.Text;

namespace Extensoes;

public static class StringExtensions
{
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
        else
        {
            cpfCnpj = cpfCnpj.PadLeft(14, '0');
            cpfCnpj = cpfCnpj.Insert(12, "-");
            cpfCnpj = cpfCnpj.Insert(8, "/");
            cpfCnpj = cpfCnpj.Insert(5, ".");
            return cpfCnpj.Insert(2, ".");
        }
    }

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

        throw new ArgumentException();
    }

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

        throw new ArgumentException();
    }

    public static string RemoveMaskCpfCnpj(this string cpfCnpj)
    {
        return cpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "").Trim();
    }

    public static string MaskRegistrationCpfCnpj(this string registration)
    {
        if (registration.Length == 8)
            return string.Format(@"{0:00\.000\.000}", double.Parse(registration));

        return string.Format(@"{0:000\.000\.000}", double.Parse(registration));
    }

    public static string TelephoneMask(this string telephone)
    {
        telephone = telephone.Trim();
        telephone = telephone.Replace(" ", "").Replace("(", "").Replace(")", "");

        if (telephone.Length == 11)
        {
            telephone = telephone.Insert(7, "-");
            telephone = telephone.Insert(2, ") ");
            telephone = telephone.Insert(0, "(");
        }
        else if (telephone.Length == 10)
        {
            telephone = telephone.Insert(6, "-");
            telephone = telephone.Insert(2, ") ");
            telephone = telephone.Insert(0, "(");
        }
        else
        {
            telephone = string.Empty;
        }
        return telephone;
    }

    public static string ZipCodeMask(this string zipCode)
    {
        zipCode = zipCode.Trim();
        zipCode = zipCode.Replace(" ", "").Replace("-", "").Replace(".", "");

        if (zipCode.Length == 8)
            return zipCode.Insert(5, "-");

        return string.Empty;
    }

    public static string ToTitleCase(this string text)
    {
        var textInfo = new CultureInfo("pt-Br", false).TextInfo;
        return textInfo.ToTitleCase(text.ToLower());
    }

    public static string IsNull(this string text)
    {
        return string.IsNullOrEmpty(text) ? string.Empty : text;
    }

    public static string ReplaceAccents(this string text)
    {
        return new string(text.Normalize(NormalizationForm.FormD)
                            .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                            .ToArray());
    }

    public static string RemoveSpaces(this string text)
    {
        return text.Replace(" ", "");
    }
}