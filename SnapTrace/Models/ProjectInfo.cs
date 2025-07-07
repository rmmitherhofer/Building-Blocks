using SnapTrace.Enums;
using System.Text.Json.Serialization;

namespace SnapTrace.Models;


/// <summary>
/// Contains basic information about a project.
/// </summary>
public class ProjectInfo
{
    /// <summary>
    /// Unique identifier of the project.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the project.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Type of the project.
    /// </summary>
    [JsonPropertyName("type")]
    public ProjectType Type { get; set; }

    /// <summary>
    /// Defines the current logging behavior of SnapTrace.
    /// </summary>
    [JsonPropertyName("executionMode")]
    public SnapTraceExecutionMode ExecutionMode { get; set; }

    /// <summary>
    /// Maximum size of the response body to capture, in megabytes.
    /// </summary>
    [JsonPropertyName("maxResponseBodySizeInMb")]
    public int MaxResponseBodySizeInMb { get; set; } = 1;

    /// <summary>
    /// Indicates whether to capture the response body for logging.
    /// </summary>
    [JsonPropertyName("captureResponseBody")]
    public bool CaptureResponseBody { get; set; } = true;

    /// <summary>
    /// Indicates whether the request and response payload should be written to the console output.
    /// Useful for debugging during development.
    /// </summary>
    [JsonPropertyName("writePayloadToConsole")]
    public bool WritePayloadToConsole { get; set; } = false;
}


