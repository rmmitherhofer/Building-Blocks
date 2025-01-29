namespace Swagger.Options;

public enum DescriptionSources
{
    /// <summary>
    /// <see cref="DescriptionAttribute"/>.
    /// </summary>
    DescriptionAttributes = 0,

    /// <summary>
    /// Xml comments.
    /// </summary>
    XmlComments = 1,

    /// <summary>
    /// <see cref="DescriptionAttribute"/> then xml comments.
    /// </summary>
    DescriptionAttributesThenXmlComments = 2
}