namespace Api.Swagger.Options;

/// <summary>
/// Various configuration properties for customizing enum representation in Swagger documentation.
/// </summary>
public class FixEnumsOptions
{
    #region Properties

    /// <summary>
    /// Gets the set of included XML documentation file paths to read comments from.
    /// </summary>
    public HashSet<string> IncludedXmlCommentsPaths { get; } = new HashSet<string>();

    /// <summary>
    /// Gets or sets a value indicating whether to include descriptions from <see cref="System.ComponentModel.DescriptionAttribute"/> or XML comments.
    /// Default is <c>false</c>.
    /// </summary>
    public bool IncludeDescriptions { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to include remarks from XML comments in enum descriptions.
    /// Default is <c>false</c>.
    /// </summary>
    public bool IncludeXEnumRemarks { get; set; } = false;

    /// <summary>
    /// Gets or sets the source to retrieve descriptions from.
    /// Default is <see cref="DescriptionSources.DescriptionAttributes"/>.
    /// </summary>
    public DescriptionSources DescriptionSource { get; set; } = DescriptionSources.DescriptionAttributes;

    /// <summary>
    /// Gets or sets a value indicating whether to apply the enum names schema filter in Swagger schemas.
    /// This corresponds to calling <c>options.SchemaFilter&lt;XEnumNamesSchemaFilter&gt;();</c>.
    /// Default is <c>true</c>.
    /// </summary>
    public bool ApplySchemaFilter { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to apply the enum names parameter filter in Swagger parameters.
    /// This corresponds to calling <c>options.ParameterFilter&lt;XEnumNamesParameterFilter&gt;();</c>.
    /// Default is <c>true</c>.
    /// </summary>
    public bool ApplyParameterFilter { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to apply the document filter to display enum values with descriptions.
    /// This corresponds to calling <c>options.DocumentFilter&lt;DisplayEnumsWithValuesDocumentFilter&gt;();</c>.
    /// Default is <c>true</c>.
    /// </summary>
    public bool ApplyDocumentFilter { get; set; } = true;

    /// <summary>
    /// Gets or sets the alias key used for enum names in the Swagger documentation extensions.
    /// Default is <c>"x-enumNames"</c>.
    /// </summary>
    public string XEnumNamesAlias { get; set; } = "x-enumNames";

    /// <summary>
    /// Gets or sets the alias key used for enum descriptions in the Swagger documentation extensions.
    /// Default is <c>"x-enumDescriptions"</c>.
    /// </summary>
    public string XEnumDescriptionsAlias { get; set; } = "x-enumDescriptions";

    /// <summary>
    /// Gets or sets the string used for new lines in enum values descriptions.
    /// For example: <see cref="Environment.NewLine"/> or "\n".
    /// Default is "\n".
    /// </summary>
    public string NewLine { get; set; } = "\n";

    #endregion
}
