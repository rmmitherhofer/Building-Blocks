using System.Text.Json.Serialization;

namespace Api.Responses;

/// <summary>
/// Represents the base class for paginated API responses, including metadata such as total records and navigation pages.
/// </summary>
public abstract class PaginatedResponse : Response
{
    /// <summary>
    /// Gets the total number of records returned by the query (not just on the current page).
    /// </summary>
    [JsonPropertyName("totalRecords")]

    public int TotalRecords { get; set; }

    /// <summary>
    /// Gets the number of records per page.
    /// </summary>
    [JsonPropertyName("pageSize")]

    public int PageSize { get; set; } = 0;

    /// <summary>
    /// Gets the current page number (1-based index).
    /// </summary>
    [JsonPropertyName("pageNumber")]

    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets the total number of pages based on the total records and page size.
    /// </summary>
    [JsonPropertyName("pageCount")]
    public int PageCount { get; set; } = 0;

    /// <summary>
    /// Gets the previous page number, if it exists; otherwise, null.
    /// </summary>
    [JsonPropertyName("backPage")]
    public int? BackPage { get; set; }

    /// <summary>
    /// Gets the next page number, if it exists; otherwise, null.
    /// </summary>
    [JsonPropertyName("nextPage")]
    public int? NextPage { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedResponse"/> class.
    /// </summary>
    protected PaginatedResponse() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedResponse"/> class with pagination metadata.
    /// </summary>
    /// <param name="totalRecords">Total number of records in the full result set.</param>
    /// <param name="pageNumber">Current page number (starting at 1).</param>
    /// <param name="pageCount">Total number of pages.</param>
    /// <param name="pageSize">Number of records per page.</param>
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
