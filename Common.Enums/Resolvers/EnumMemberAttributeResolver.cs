using System.Reflection;
using System.Runtime.Serialization;

namespace Common.Enums.Resolvers;

/// <summary>
/// Resolves the description of an enum member using the <see cref="EnumMemberAttribute.Value"/>.
/// </summary>
public class EnumMemberAttributeResolver : IEnumDescriptionResolver
{
    /// <summary>
    /// Gets the description from the <see cref="EnumMemberAttribute.Value"/> applied to the enum field.
    /// </summary>
    /// <param name="field">The field info representing the enum member.</param>
    /// <returns>
    /// The value defined by the <see cref="EnumMemberAttribute"/>, or <c>null</c> if not present.
    /// </returns>
    public string? GetDescription(FieldInfo field) =>
        field.GetCustomAttribute<EnumMemberAttribute>()?.Value;
}
