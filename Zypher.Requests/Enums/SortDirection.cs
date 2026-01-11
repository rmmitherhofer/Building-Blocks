namespace Zypher.Requests.Enums;

/// <summary>
/// Specifies the direction used when sorting query results.
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// Sorts the results in ascending order.
    /// </summary>
    /// <remarks>
    /// Example: A → Z, 0 → 9, older dates → newer dates.
    /// </remarks>
    Asc,

    /// <summary>
    /// Sorts the results in descending order.
    /// </summary>
    /// <remarks>
    /// Example: Z → A, 9 → 0, newer dates → older dates.
    /// </remarks>
    Desc
}
