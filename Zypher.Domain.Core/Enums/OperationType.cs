namespace Zypher.Domain.Core.Enums;

/// <summary>
/// Defines the types of operations that can be performed on an entity.
/// </summary>
public enum OperationType
{
    /// <summary>
    /// No operation.
    /// </summary>
    None = 0,

    /// <summary>
    /// Entity was added.
    /// </summary>
    Added = 1,

    /// <summary>
    /// Entity was modified.
    /// </summary>
    Modified = 2,

    /// <summary>
    /// Entity was deleted.
    /// </summary>
    Deleted = 3,
}
