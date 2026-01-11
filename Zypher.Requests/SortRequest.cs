using Zypher.Requests.Enums;

namespace Zypher.Requests;

/// <summary>
/// Represents a sorting instruction for a query request.
/// </summary>
/// <remarks>
/// Multiple sort instructions can be combined to define
/// primary, secondary, and subsequent ordering rules.
/// </remarks>
public class SortRequest
{
    /// <summary>
    /// Gets or sets the field name used for sorting.
    /// </summary>
    /// <remarks>
    /// The field name must match a valid property or column
    /// exposed by the queryable data source.
    /// </remarks>
    public string Field { get; set; } = default!;

    /// <summary>
    /// Gets or sets the sort direction.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="SortDirection.Asc"/>.
    /// </remarks>
    public SortDirection Direction { get; set; } = SortDirection.Asc;
}
