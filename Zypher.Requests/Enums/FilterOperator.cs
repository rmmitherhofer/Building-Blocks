namespace Zypher.Requests.Enums;

/// <summary>
/// Defines the available comparison operators used in dynamic query filters.
/// </summary>
public enum FilterOperator
{
    /// <summary>
    /// Checks whether the field value is equal to the specified value.
    /// </summary>
    Equals,

    /// <summary>
    /// Checks whether the field value is different from the specified value.
    /// </summary>
    NotEquals,

    /// <summary>
    /// Checks whether the field value contains the specified value.
    /// Typically used for string comparisons.
    /// </summary>
    Contains,

    /// <summary>
    /// Checks whether the field value starts with the specified value.
    /// Typically used for string comparisons.
    /// </summary>
    StartsWith,

    /// <summary>
    /// Checks whether the field value ends with the specified value.
    /// Typically used for string comparisons.
    /// </summary>
    EndsWith,

    /// <summary>
    /// Checks whether the field value is greater than the specified value.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Checks whether the field value is greater than or equal to the specified value.
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Checks whether the field value is less than the specified value.
    /// </summary>
    LessThan,

    /// <summary>
    /// Checks whether the field value is less than or equal to the specified value.
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Checks whether the field value exists within a specified collection of values.
    /// </summary>
    /// <remarks>
    /// Commonly translated to SQL <c>IN</c>.
    /// </remarks>
    In,

    /// <summary>
    /// Checks whether the field value does not exist within a specified collection of values.
    /// </summary>
    /// <remarks>
    /// Commonly translated to SQL <c>NOT IN</c>.
    /// </remarks>
    NotIn,

    /// <summary>
    /// Checks whether the field value is null.
    /// </summary>
    IsNull,

    /// <summary>
    /// Checks whether the field value is not null.
    /// </summary>
    IsNotNull
}
