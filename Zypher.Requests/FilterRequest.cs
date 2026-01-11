namespace Zypher.Requests;

public abstract class FilterRequest : Request
{
    /// <summary>
    /// The page number for the query (pagination).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// The number of records per page for the query (pagination).
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Order by property name. Use prefix '-' for descending or '+' for ascending order.
    /// Example: "-createdDate" for descending or "+name" for ascending.
    /// </summary>
    public string? OrderBy { get; set; }
}