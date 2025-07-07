namespace Api.Swagger.Options;

/// <summary>
/// Represents a custom HTTP header option for Swagger configuration.
/// </summary>
public class CustomHeaderOption
{
    /// <summary>
    /// Gets or sets the name of the HTTP header.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the header is required.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets the description of the header, used for documentation purposes.
    /// </summary>
    public string Description { get; set; }
}
