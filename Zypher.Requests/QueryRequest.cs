namespace Zypher.Requests;

/// <summary>
/// Represents a generic and extensible query request used for data retrieval operations.
/// </summary>
/// <remarks>
/// This model is designed to support advanced querying scenarios, including pagination,
/// sorting, dynamic filtering, and future extensibility without breaking compatibility.
/// </remarks>
/// <remarks>
/// Execution order:
/// 1. Filter
/// 2. Sort
/// 3. Pagination
/// </remarks>
public class QueryRequest
{
    /// <summary>
    /// Defines a dynamic filter tree used to restrict the result set.
    /// </summary>
    /// <remarks>
    /// Supports nested logical groups (AND / OR) and multiple comparison operators.
    /// </remarks>
    public FilterGroup? Filter { get; set; }
    /// <summary>
    /// Defines one or more sorting rules applied to the result set.
    /// </summary>
    /// <remarks>
    /// Sorting is applied before pagination. Multiple fields can be combined.
    /// </remarks>
    public List<SortRequest> Sort { get; set; } = [];
    /// <summary>
    /// Defines pagination settings such as page number and page size.
    /// </summary>
    /// <remarks>
    /// Pagination is applied after filtering and sorting.
    /// </remarks>
    public PaginationRequest Pagination { get; set; } = new();
    /// <summary>
    /// Specifies related entities or data paths to be included in the query result.
    /// </summary>
    /// <remarks>
    /// This property is optional and may be ignored depending on the underlying data source.
    /// </remarks>
    public List<string>? Includes { get; set; }
    /// <summary>
    /// Stores additional metadata associated with the query request.
    /// </summary>
    /// <remarks>
    /// This field can be used for custom extensions, contextual information,
    /// or cross-cutting concerns such as multi-tenancy, auditing, or feature flags.
    /// </remarks>
    public Dictionary<string, object>? Metadata { get; set; }
}
