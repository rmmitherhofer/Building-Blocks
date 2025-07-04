using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json.Serialization;

namespace SnapTrace.Models;

public class LogContextRequest
{
    [JsonPropertyName("ProjectInfo")]
    public ProjectInfo ProjectInfo { get; set; }

    [JsonPropertyName("Environment")]
    public EnvironmentInfo Environment { get; set; }

    [JsonPropertyName("EndpointPath")]
    public string EndpointPath { get; set; }

    [JsonPropertyName("RequestDuration")]
    public string RequestDuration { get; set; }

    [JsonPropertyName("ElapsedMilliseconds")]
    public long ElapsedMilliseconds { get; set; }

    [JsonPropertyName("LogSeverity")]
    public int LogSeverity { get; set; }

    [JsonPropertyName("TraceIdentifier")]
    public string TraceIdentifier { get; set; }

    [JsonPropertyName("UserContextRequest")]
    public UserContextRequest UserContext { get; set; }

    [JsonPropertyName("RequestInfoRequest")]
    public RequestInfoRequest RequestInfo { get; set; }

    [JsonPropertyName("ResponseInfoRequest")]
    public ResponseInfoRequest ResponseInfo { get; set; }

    [JsonPropertyName("DiagnosticRequest")]
    public DiagnosticRequest Diagnostics { get; set; }

    [JsonPropertyName("ErrorCategory")]
    public string ErrorCategory { get; set; }

    [JsonPropertyName("LogEntries")]
    public IEnumerable<NotificationInfoRequest> Notifications { get; set; }

    [JsonPropertyName("LogEntries")]
    public IEnumerable<LogEntryRequest> LogEntries { get; set; }
    [JsonPropertyName("ExceptionDetails")]
    public IEnumerable<ExceptionInfoRequest> Exceptions { get; set; }
}
public class UserContextRequest
{
    [JsonPropertyName("UserId")]
    public string Id { get; set; }

    [JsonPropertyName("UserCode")]
    public string Code { get; set; }

    [JsonPropertyName("UserName")]
    public string Name { get; set; }

    [JsonPropertyName("IsAuthenticated")]
    public bool IsAuthenticated { get; set; }

    [JsonPropertyName("AuthenticationType")]
    public string AuthenticationType { get; set; }

    [JsonPropertyName("Roles")]
    public List<string> Roles { get; set; }

    [JsonPropertyName("Claims")]
    public Dictionary<string, string> Claims { get; set; }

    [JsonPropertyName("MachineName")]
    public string MachineName { get; set; }

    [JsonPropertyName("ClientIp")]
    public string IpAddress { get; set; }
}

public class RequestInfoRequest
{
    [JsonPropertyName("RequestId")]
    public string RequestId { get; set; }

    [JsonPropertyName("CorrelationId")]
    public string CorrelationId { get; set; }

    [JsonPropertyName("HttpMethod")]
    public string HttpMethod { get; set; }

    [JsonPropertyName("RequestUrl")]
    public string RequestUrl { get; set; }

    [JsonPropertyName("Scheme")]
    public string Scheme { get; set; }

    [JsonPropertyName("Protocol")]
    public string Protocol { get; set; }

    [JsonPropertyName("IsHttps")]
    public bool IsHttps { get; set; }

    [JsonPropertyName("QueryString")]
    public string QueryString { get; set; }

    [JsonPropertyName("RouteValues")]
    public Dictionary<string, string> RouteValues { get; set; }

    [JsonPropertyName("UserAgent")]
    public string UserAgent { get; set; }

    [JsonPropertyName("ClientId")]
    public string ClientId { get; set; }

    [JsonPropertyName("Headers")]
    public Dictionary<string, List<string>> Headers { get; set; }

    [JsonPropertyName("ContentType")]
    public string ContentType { get; set; }

    [JsonPropertyName("ContentLength")]
    public long? ContentLength { get; set; }

    [JsonPropertyName("Body")]
    public object Body { get; set; }

    [JsonPropertyName("BodySize")]
    public double BodySize { get; set; }
}

public class ResponseInfoRequest

{
    [JsonPropertyName("StatusCode")]
    public HttpStatusCode StatusCode { get; set; }

    [JsonPropertyName("ReasonPhrase")]
    public string ReasonPhrase { get; set; }

    [JsonPropertyName("Headers")]
    public Dictionary<string, List<string>> Headers { get; set; }

    [JsonPropertyName("Body")]
    public object Body { get; set; }

    [JsonPropertyName("BodySize")]
    public double BodySize { get; set; }
}

public class DiagnosticRequest
{
    [JsonPropertyName("MemoryUsageMb")]
    public double MemoryUsageMb { get; set; }

    [JsonPropertyName("DbQueryCount")]
    public int DbQueryCount { get; set; }

    [JsonPropertyName("CacheHit")]
    public bool CacheHit { get; set; }

    [JsonPropertyName("Dependencies")]
    public List<DependencyInfoRequest> Dependencies { get; set; }
}

public class DependencyInfoRequest
{
    [JsonPropertyName("Type")]
    public string Type { get; set; }

    [JsonPropertyName("Target")]
    public string Target { get; set; }

    [JsonPropertyName("Success")]
    public bool Success { get; set; }

    [JsonPropertyName("DurationMs")]
    public int DurationMs { get; set; }
}

public class LogEntryRequest
{
    [JsonPropertyName("LogCategory")]
    public string LogCategory { get; set; }

    [JsonPropertyName("LogSeverity")]
    public LogLevel LogSeverity { get; set; }

    [JsonPropertyName("LogMessage")]
    public string LogMessage { get; set; }

    [JsonPropertyName("MemberType")]
    public string? MemberType { get; set; }

    [JsonPropertyName("MemberName")]
    public string? MemberName { get; set; }

    [JsonPropertyName("SourceLineNumber")]
    public int SourceLineNumber { get; set; }

    [JsonPropertyName("Timestamp")]
    public DateTime Timestamp { get; set; }
}

public class ExceptionInfoRequest
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
    [JsonPropertyName("tracer")]
    public string? Tracer { get; set; }
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }
}

public class  NotificationInfoRequest
{
    public Guid Id { get;  set; }

    public DateTime Timestamp { get;  set; }

    public LogLevel LogLevel { get;  set; }

    public string? Key { get;  set; }

    public string Value { get;  set; }

    public string? Detail { get;  set; }
}