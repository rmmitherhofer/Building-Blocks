namespace Api.Core.Data.Filters;

public abstract class Filter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? OrderBy { get; set; }

    public int GetPageCount(int totalRecords) 
        => (int)Math.Ceiling(totalRecords / (double)PageSize);
}
