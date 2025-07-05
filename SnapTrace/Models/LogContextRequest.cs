using SnapTrace.Enums;
using System.Text.Json.Serialization;

namespace SnapTrace.Models;


/// <summary>
/// Represents the full context of a request log, including environment, request, response, diagnostics, and exceptions.
/// </summary>
public class LogContextRequest
{
    /// <summary>
    /// Information about the project generating the log.
    /// </summary>
    [JsonPropertyName("projectInfo")]
    public ProjectInfo ProjectInfo { get; set; }

    /// <summary>
    /// Information about the environment where the application is running.
    /// </summary>
    [JsonPropertyName("environment")]
    public EnvironmentInfo Environment { get; set; }

    /// <summary>
    /// The HTTP endpoint path requested.
    /// </summary>
    [JsonPropertyName("endpointPath")]
    public string EndpointPath { get; set; }

    /// <summary>
    /// Formatted duration of the request.
    /// </summary>
    [JsonPropertyName("requestDuration")]
    public string RequestDuration { get; set; }

    /// <summary>
    /// Elapsed time of the request in milliseconds.
    /// </summary>
    [JsonPropertyName("elapsedMilliseconds")]
    public long ElapsedMilliseconds { get; set; }

    /// <summary>
    /// Attention level of the log.
    /// </summary>
    [JsonPropertyName("logAttentionLevel")]
    public LogAttentionLevel LogAttentionLevel { get; set; }

    /// <summary>
    /// Unique identifier for the request trace.
    /// </summary>
    [JsonPropertyName("traceIdentifier")]
    public string TraceIdentifier { get; set; }

    /// <summary>
    /// Authenticated user information, if available.
    /// </summary>
    [JsonPropertyName("userContext")]
    public UserContextRequest UserContext { get; set; }

    /// <summary>
    /// HTTP request information.
    /// </summary>
    [JsonPropertyName("requestInfo")]
    public RequestInfoRequest RequestInfo { get; set; }

    /// <summary>
    /// HTTP response information.
    /// </summary>
    [JsonPropertyName("responseInfo")]
    public ResponseInfoRequest ResponseInfo { get; set; }

    /// <summary>
    /// Diagnostic data such as memory usage, dependencies, and cache hits.
    /// </summary>
    [JsonPropertyName("diagnostics")]
    public DiagnosticRequest Diagnostics { get; set; }

    /// <summary>
    /// Category of the detected error, if applicable.
    /// </summary>
    [JsonPropertyName("errorCategory")]
    public string ErrorCategory { get; set; }

    /// <summary>
    /// Notifications generated during the request processing.
    /// </summary>
    [JsonPropertyName("notifications")]
    public IEnumerable<NotificationInfoRequest> Notifications { get; set; }

    /// <summary>
    /// Detailed log entries (messages, levels, sources).
    /// </summary>
    [JsonPropertyName("logEntries")]
    public IEnumerable<LogEntryRequest> LogEntries { get; set; }

    /// <summary>
    /// Captured exception details during processing.
    /// </summary>
    [JsonPropertyName("exceptionDetails")]
    public IEnumerable<ExceptionInfoRequest> Exceptions { get; set; }
}
