using System.ComponentModel;

namespace Api.Swagger.Options;

/// <summary>
/// Specifies the source of enum value descriptions.
/// </summary>
public enum DescriptionSources
{
    /// <summary>
    /// Use <see cref="DescriptionAttribute"/> to get the description.
    /// </summary>
    DescriptionAttributes = 0,

    /// <summary>
    /// Use XML comments to get the description.
    /// </summary>
    XmlComments = 1,

    /// <summary>
    /// Use <see cref="DescriptionAttribute"/> first, then fallback to XML comments if none found.
    /// </summary>
    DescriptionAttributesThenXmlComments = 2
}
