namespace Api.Swagger.Options;

/// <summary>
/// Configuration options for default Swagger HTTP headers.
/// </summary>
public class SwaggerDefaultHeadersOptions
{
    /// <summary>
    /// Configuration for the "X-Client-ID" header.
    /// </summary>
    public HeaderOption XClientId { get; set; } = new HeaderOption
    {
        Enabled = true,
        Required = true,
        Description = "Client ID, used to identify the client application making the request."
    };

    /// <summary>
    /// Configuration for the "X-Forwarded-For" header.
    /// </summary>
    public HeaderOption XForwardedFor { get; set; } = new HeaderOption
    {
        Enabled = true,
        Required = false,
        Description = "The original IP address of the client making the request."
    };

    /// <summary>
    /// Configuration for the "X-Correlation-ID" header.
    /// </summary>
    public HeaderOption XCorrelationId { get; set; } = new HeaderOption
    {
        Enabled = true,
        Required = false,
        Description = "A unique identifier for tracing requests across systems."
    };

    /// <summary>
    /// Configuration for the "User-Agent" header.
    /// </summary>
    public HeaderOption UserAgent { get; set; } = new HeaderOption
    {
        Enabled = true,
        Required = false,
        Description = "Information about the client application making the request."
    };

    /// <summary>
    /// Collection of additional custom headers to include.
    /// </summary>
    public List<CustomHeaderOption> AdditionalHeaders { get; set; } = new List<CustomHeaderOption>();
}
