namespace Api.Responses;
public abstract class PaginatedResponse : Response
{
    /// <summary>
    /// Total de registros encontrados
    /// </summary>
    public int TotalRecords { get; }
    /// <summary>
    /// Quantidade de registros por consulta/paginas
    /// </summary>
    public int PageSize { get; } = 0;
    /// <summary>
    /// Numero da pagina/consulta
    /// </summary>
    public int PageNumber { get; } = 1;
    /// <summary>
    /// Total de paginas/consultas
    /// </summary>
    public int PageCount { get; } = 0;
    /// <summary>
    /// Pagina/consulta anterior
    /// </summary>
    public int? BackPage { get; }
    /// <summary>
    /// Proxima pagina/consulta
    /// </summary>
    public int? NextPage { get; }

    protected PaginatedResponse(int totalRecords, int pageNumber, int pageCount, int pageSize)
    {
        if (pageNumber > 1)
            BackPage = pageNumber - 1;

        if (pageCount > 0 && pageNumber != pageCount)
            NextPage = pageNumber + 1;

        PageNumber = pageNumber;
        PageCount = pageCount;

        PageSize = pageSize;

        TotalRecords = totalRecords;
    }

}
