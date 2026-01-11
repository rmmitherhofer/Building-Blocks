namespace Zypher.Requests;

/// <summary>
/// Represents pagination parameters for a query request.
/// </summary>
/// <remarks>
/// Pagination is applied after filtering and sorting operations.
/// Default values are provided to ensure predictable behavior.
/// </remarks>
public class PaginationRequest
{
    /// <summary>
    /// Gets or sets the current page number.
    /// </summary>
    /// <remarks>
    /// The first page is represented by value <c>1</c>.
    /// </remarks>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of records returned per page.
    /// </summary>
    /// <remarks>
    /// This value should be limited by the server to prevent excessive data retrieval.
    /// </remarks>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Indicates whether the total number of records should be calculated and returned.
    /// </summary>
    /// <remarks>
    /// When enabled, the query execution may require an additional count operation,
    /// which can impact performance on large data sets.
    /// </remarks>
    public bool IncludeTotalCount { get; set; } = true;
}
