using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Responses;

/// <summary>
/// Classe Response de registro nao localizado
/// </summary>
public class NotFoundResponse : NotFoundResult
{
    /// <summary>
    /// Status code
    /// </summary>
    public HttpStatusCode Status { get; }
    /// <summary>
    /// Titulo
    /// </summary>
    public string Title { get; }
    /// <summary>
    /// Datalhamento
    /// </summary>
    public string Detail { get; }
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="detail"></param>
    public NotFoundResponse(string? detail = null)
    {
        Status = HttpStatusCode.NotFound;
        Title = "Não encontrado";
        Detail = detail ?? "Consulta não retornou registros";
    }
}