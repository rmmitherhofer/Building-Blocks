namespace Extensoes;

public static class CoinExtensions
{
    public static string ToReal(this decimal? value) => $"R$ {value.PtBR()}";

    public static string? RemoveMaskReal(this string? value) => value?.Replace("R$", "")?.Trim();
}