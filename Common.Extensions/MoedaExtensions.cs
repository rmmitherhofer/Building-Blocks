namespace Extensoes;

public static class MoedaExtensions
{
    public static string ParaReais(this decimal? value) => $"R$ {value.PtBR()}";

    public static string? RemoverMascaraReais(this string? value) => value?.Replace("R$", "")?.Trim();
}