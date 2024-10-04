using System.Globalization;

namespace Extensoes;

public static class NumberExtensions
{
    public static string PtBR(this decimal? value)
    {
        if (value == null) return "0";
        return FormatarNumeroParaPtBR(value);
    }

    public static string PtBR(this decimal value) => FormatarNumeroParaPtBR(value);

    private static string FormatarNumeroParaPtBR(object value, int quantidadeDecimais = 2)
    {
        if (string.IsNullOrWhiteSpace(value?.ToString())) throw new ArgumentNullException(nameof(value));

        return string.Format(new CultureInfo("pt-BR"), $"{{0:N{quantidadeDecimais}}}", value);
    }

    public static decimal? ReverterMascaraParaDecimal(this string? value, CultureInfo? culture = null)
    {
        if (string.IsNullOrEmpty(value)) return null;

        bool converteu = decimal.TryParse(ReverterMascara(value, culture), out decimal result);

        return converteu ? result : throw new FormatException($"Falha ao formatar valor {value}");
    }

    private static string ReverterMascara(string value, CultureInfo? culture = null)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

        string valorConvertido = value.RemoverMascaraReais().Replace(" ", "").Trim();

        culture ??= CultureInfo.CurrentCulture;

        if (!valorConvertido.Contains(','))
            return valorConvertido.Replace(".", "");

        string newValue = string.Empty;

        var num = valorConvertido.Replace(",", ".").Split(".");

        for (int i = 0; i < num.Length; i++)
        {
            bool ehUltimoValor = num.Length - 1 == i;

            if (ehUltimoValor)
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
