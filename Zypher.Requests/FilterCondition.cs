using Zypher.Requests.Enums;

namespace Zypher.Requests;

/// <summary>
/// Represents a single filter condition applied to a query.
/// </summary>
/// <remarks>
/// A filter condition defines a comparison between a field
/// and a value using a specific operator.
/// </remarks>
public class FilterCondition
{
    /// <summary>
    /// Gets or sets the field name to be evaluated by the filter.
    /// </summary>
    /// <remarks>
    /// The field name must match a valid property or column
    /// exposed by the queryable data source.
    /// </remarks>
    public string Field { get; set; } = default!;

    /// <summary>
    /// Gets or sets the operator used to compare the field and the value.
    /// </summary>
    public FilterOperator Operator { get; set; }

    /// <summary>
    /// Gets or sets the value used for the comparison.
    /// </summary>
    /// <remarks>
    /// The value type must be compatible with the target field.
    /// For operators such as <c>In</c> or <c>Between</c>, this value
    /// may represent a collection or a range object.
    /// </remarks>
    public object? Value { get; set; }
}
