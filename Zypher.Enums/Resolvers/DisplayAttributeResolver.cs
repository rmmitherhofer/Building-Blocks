using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Zypher.Enums.Resolvers;

/// <summary>
/// Resolves the description of an enum member using the <see cref="DisplayAttribute.Name"/>.
/// </summary>
public class DisplayAttributeResolver : IEnumDescriptionResolver
{
    /// <summary>
    /// Gets the description from the <see cref="DisplayAttribute.Name"/> applied to the enum field.
    /// </summary>
    /// <param name="field">The field info representing the enum member.</param>
    /// <returns>
    /// The name defined by the <see cref="DisplayAttribute"/>, or <c>null</c> if not present.
    /// </returns>
    public string? GetDescription(FieldInfo field) =>
        field.GetCustomAttribute<DisplayAttribute>()?.Name;
}
