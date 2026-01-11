namespace Zypher.Requests.Enums;

/// <summary>
/// Defines the logical operators used to combine filter conditions and groups.
/// </summary>
public enum FilterLogicalOperator
{
    /// <summary>
    /// Combines conditions or groups using a logical AND operation.
    /// All conditions must be satisfied.
    /// </summary>
    And,

    /// <summary>
    /// Combines conditions or groups using a logical OR operation.
    /// At least one condition must be satisfied.
    /// </summary>
    Or
}