using System.Globalization;

namespace Extensoes;

public static class NumberExtensions
{
    public static string PtBR(this decimal? value)
    {
        if (value == null) return "0";
        return FormatNumberToPtBR(value);
    }

    public static string PtBR(this decimal value) => FormatNumberToPtBR(value);

    private static string FormatNumberToPtBR(object value, int decimalQuantity = 2)
    {
        if (string.IsNullOrWhiteSpace(value?.ToString())) throw new ArgumentNullException(nameof(value));

        return string.Format(new CultureInfo("pt-BR"), $"{{0:N{decimalQuantity}}}", value);
    }

    public static decimal? RevertMaskToDecimal(this string? value, CultureInfo? culture = null)
    {
        if (string.IsNullOrEmpty(value)) return null;

        bool converted = decimal.TryParse(RevertMask(value, culture), out decimal result);

        return converted ? result : throw new FormatException($"Falha ao formatar valor {value}");
    }

    private static string RevertMask(string value, CultureInfo? culture = null)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

        string convertedValue = value.RemoveMaskReal().Replace(" ", "").Trim();

        culture ??= CultureInfo.CurrentCulture;

        if (!convertedValue.Contains(','))
            return convertedValue.Replace(".", "");

        string newValue = string.Empty;

        var num = convertedValue.Replace(",", ".").Split(".");

        for (int i = 0; i < num.Length; i++)
        {
            bool isLastValue = num.Length - 1 == i;

            if (isLastValue)
            {
                newValue += culture.Name switch
                {
                    "pt-BR" => $",{num[i]}",
                    _ => $".{num[i]}"
                };
            }
            else
            {
                newValue += num[i];
            }
        }

        return newValue;
    }

}
