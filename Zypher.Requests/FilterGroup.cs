using Zypher.Requests.Enums;

namespace Zypher.Requests;

/// <summary>
/// Represents a group of filter conditions that can be combined
/// using a logical operator (AND / OR).
/// </summary>
/// <remarks>
/// A filter group allows the creation of complex and nested
/// filter expressions, such as:
/// <code>
/// (A AND B) OR (C AND (D OR E))
/// </code>
/// </remarks>
public class FilterGroup
{
    /// <summary>
    /// Gets or sets the logical operator used to combine
    /// the conditions and nested groups.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="FilterLogicalOperator.And"/>.
    /// </remarks>
    public FilterLogicalOperator LogicalOperator { get; set; } = FilterLogicalOperator.And;

    /// <summary>
    /// Gets or sets the list of filter conditions applied
    /// within this group.
    /// </summary>
    public List<FilterCondition> Conditions { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of nested filter groups.
    /// </summary>
    /// <remarks>
    /// Nested groups allow building hierarchical filter trees
    /// with different logical operators at each level.
    /// </remarks>
    public List<FilterGroup> Groups { get; set; } = [];
}
