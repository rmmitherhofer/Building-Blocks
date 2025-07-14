namespace Zypher.Persistence.Abstractions.Data.Filters;

/// <summary>
/// Base class for filtering data with pagination and sorting.
/// </summary>
public abstract class Filter
{
    /// <summary>
    /// Gets or sets the current page number (starting at 1).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of records per page.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the ordering expression.
    /// Use "-" prefix for descending or "+" prefix for ascending order.
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Calculates the total number of pages based on total records.
    /// </summary>
    /// <param name="totalRecords">Total number of records.</param>
    /// <returns>The total number of pages.</returns>
    public int GetPageCount(int totalRecords)
        => (int)Math.Ceiling(totalRecords / (double)PageSize);
}
