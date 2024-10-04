namespace Api.Requests;

public abstract class FilterRequest : Request
{
    /// <summary>
    /// Numero da consulta
    /// </summary>
    public int PageNumber { get; set; } = 1;
    /// <summary>
    /// Quantidade de registros por consulta
    /// </summary>
    public int PageSize { get; set; } = 10;
    /// <summary>
    /// Ordenar por | utilize antes da propriedade os sinais (-) para ordenar do maior para o menor ou (+) para ordenar do menor para o maior
    /// </summary>
    public string? OrderBy { get; set; }
}
