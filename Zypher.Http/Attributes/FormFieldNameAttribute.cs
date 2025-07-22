namespace Zypher.Http.Attributes;

/// <summary>
/// Specifies a custom form field name to be used when serializing a property
/// into multipart/form-data content.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class FormFieldNameAttribute : Attribute
{
    /// <summary>
    /// Gets the custom form field name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FormFieldNameAttribute"/> class
    /// with the specified form field name.
    /// </summary>
    /// <param name="name">The custom form field name to use during serialization.</param>

    public FormFieldNameAttribute(string name)
    {
        Name = name;
    }
}