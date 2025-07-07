using System.ComponentModel;
using System.Reflection;

namespace Common.Enums.Resolvers;

/// <summary>
/// Resolves the description of an enum member using the <see cref="DescriptionAttribute"/>.
/// </summary>
public class DescriptionAttributeResolver : IEnumDescriptionResolver
{
    /// <summary>
    /// Gets the description from the <see cref="DescriptionAttribute"/> applied to the enum field.
    /// </summary>
    /// <param name="field">The field info representing the enum member.</param>
    /// <returns>
    /// The description defined by the <see cref="DescriptionAttribute"/>, or <c>null</c> if not present.
    /// </returns>
    public string? GetDescription(FieldInfo field) =>
        field.GetCustomAttribute<DescriptionAttribute>()?.Description;
}
