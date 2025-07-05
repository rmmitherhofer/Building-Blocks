using SnapTrace.Enums;
using System.Reflection;

namespace SnapTrace.Configurations.Settings;

/// <summary>
/// Main configuration settings for SnapTrace, including project identification,
/// type, name, and logging parameters.
/// </summary>
public class SnapTraceSettings
{
    /// <summary>
    /// Unique identifier for the project.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Type of the project (defined by the <see cref="ProjectType"/> enum).
    /// </summary>
    public ProjectType ProjectType { get; set; }

    /// <summary>
    /// Name of the project, automatically obtained from the entry assembly.
    /// </summary>
    public string Name { get; } = Assembly.GetEntryAssembly().GetName().Name;

    /// <summary>
    /// Indicates whether logging is enabled.
    /// </summary>
    public bool TurnOnLog { get; set; }

    /// <summary>
    /// Maximum size of the response body to capture, in megabytes.
    /// </summary>
    public int MaxResponseBodySizeInMb { get; set; } = 1;

    /// <summary>
    /// HTTP service specific settings for SnapTrace.
    /// </summary>
    public SnapTraceHttpServiceSettings Service { get; set; }

    /// <summary>
    /// Indicates whether the request and response payload should be written to the console output.
    /// Useful for debugging during development.
    /// </summary>
    public bool WritePayloadToConsole { get; set; } = false;
}

/// <summary>
/// HTTP service configuration used by SnapTrace.
/// </summary>
public class SnapTraceHttpServiceSettings
{
    /// <summary>
    /// Base address of the HTTP service.
    /// </summary>
    public string BaseAddress { get; set; }

    /// <summary>
    /// Endpoint configurations available on the service.
    /// </summary>
    public SnapTraceHttpServiceEndPointsSettings EndPoints { get; set; }
}

/// <summary>
/// Endpoint configurations for the SnapTrace HTTP service.
/// </summary>
public class SnapTraceHttpServiceEndPointsSettings
{
    /// <summary>
    /// Endpoint used for sending notifications.
    /// </summary>
    public string Notify { get; set; }
}
