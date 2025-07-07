namespace Api.Swagger.Options;

/// <summary>
/// Represents configuration options for an HTTP header.
/// </summary>
public class HeaderOption
{
    /// <summary>
    /// Gets or sets a value indicating whether this header is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this header is required.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets the description for this header.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
