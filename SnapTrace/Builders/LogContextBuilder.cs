using Common.Extensions;
using Common.Json;
using Common.Notifications.Interfaces;
using Common.Notifications.Messages;
using Common.User.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnapTrace.Adapters;
using SnapTrace.Configurations.Settings;
using SnapTrace.Extensions;
using SnapTrace.Models;
using System.Net;
using System.Reflection;

namespace SnapTrace.Builders;

/// <summary>
/// Builder for the log context that collects detailed information about the HTTP request,
/// response, environment, user, notifications, exceptions, and other metrics for sending to SnapTrace.
/// </summary>
public class LogContextBuilder : ILogContextBuilder
{
    private readonly SnapTraceSettings _settings;
    private readonly SensitiveDataMasker _sensitiveDataMasker;
    private readonly INotificationHandler _notification;

    private HttpContext _context;
    private LogLevel _logLevel;
    private long _elapsedMs;
    private string? _errorCategory;
    private IEnumerable<Notification> _notifications = [];
    private Exception? _exception;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogContextBuilder"/> class.
    /// </summary>
    /// <param name="options">SnapTrace configuration settings.</param>
    /// <param name="sensitiveDataMasker">Service for masking sensitive data.</param>
    /// <param name="notification">Handler for system notifications.</param>
    public LogContextBuilder(IOptions<SnapTraceSettings> options, SensitiveDataMasker sensitiveDataMasker, INotificationHandler notification)
    {
        _settings = options.Value;
        _sensitiveDataMasker = sensitiveDataMasker;
        _notification = notification;
    }

    /// <summary>
    /// Sets the current HTTP context to capture data from.
    /// </summary>
    /// <param name="context">HTTP context of the request.</param>
    /// <returns>The builder instance for chaining.</returns>
    public ILogContextBuilder WithHttpContext(HttpContext context)
    {
        _context = context;
        return this;
    }

    /// <summary>
    /// Sets the elapsed time of the request in milliseconds.
    /// </summary>
    /// <param name="elapsedMs">Elapsed time in milliseconds.</param>
    /// <returns>The builder instance for chaining.</returns>
    public ILogContextBuilder WithElapsedMilliseconds(long elapsedMs)
    {
        _elapsedMs = elapsedMs;
        return this;
    }

    /// <summary>
    /// Includes the current system notifications in the log context.
    /// </summary>
    /// <returns>The builder instance for chaining.</returns>
    public ILogContextBuilder WithNotifications()
    {
        _notifications = _notification.Get() ?? [];
        return this;
    }

    /// <summary>
    /// Includes the captured exception in the log context, if any.
    /// </summary>
    /// <returns>The builder instance for chaining.</returns>
    public ILogContextBuilder WithException()
    {
        _exception = GetException();
        return this;
    }

    /// <summary>
    /// Builds the <see cref="LogContextRequest"/> object containing all collected data for sending.
    /// </summary>
    /// <returns>A detailed object representing the log context.</returns>
    public async Task<LogContextRequest> BuildAsync()
    {
        return new LogContextRequest
        {
            EndpointPath = _context.Request.Path,
            RequestDuration = _elapsedMs.GetTime(),
            ElapsedMilliseconds = _elapsedMs,
            TraceIdentifier = _context.TraceIdentifier,
            ProjectInfo = AddProject(),
            Environment = AddEnvironment(),
            UserContext = AddUserContext(),
            RequestInfo = await AddRequestInfoAsync(),
            ResponseInfo = AddResponseInfo(),
            Diagnostics = AddDiagnostics(),
            LogEntries = AddLogEntries(),
            ErrorCategory = _errorCategory,
            Notifications = AddNotifications(),
            Exceptions = AddExceptions(),
            LogAttentionLevel = _logLevel.Map(),
        };
    }

    /// <summary>
    /// Adds information about the configured project.
    /// </summary>
    private ProjectInfo AddProject() => new()
    {
        Id = _settings.ProjectId,
        Name = _settings.Name,
        Type = _settings.ProjectType
    };

    /// <summary>
    /// Adds information about the current execution environment.
    /// </summary>
    private EnvironmentInfo AddEnvironment() => new()
    {
        MachineName = Environment.MachineName,
        EnvironmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
        ApplicationVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "Unknown",
        ThreadId = Environment.CurrentManagedThreadId,
    };

    /// <summary>
    /// Adds information about the current user's context.
    /// </summary>
    private UserContextRequest AddUserContext()
    {
        var user = _context.User;
        var request = _context.Request;

        return new()
        {
            Id = user.GetId() ?? request.GetUserId(),
            TenantId = user.GetTenantId(),
            Name = user.GetName() ?? request.GetUserName(),
            IsAuthenticated = user.IsAuthenticated(),
            AuthenticationType = user.GetAuthenticationType(),
            Roles = user.GetRoles(),
            Claims = user.Claims.ToDictionary(c => c.Type, c => c.Value),
            IpAddress = request.GetIpAddress()
        };
    }

    /// <summary>
    /// Adds detailed HTTP request information, including headers and message body.
    /// </summary>
    /// <returns>Object containing request data.</returns>
    private async Task<RequestInfoRequest> AddRequestInfoAsync()
    {
        var request = _context.Request;
        var body = await GetRequestBodyAsync();

        return new()
        {
            RequestId = request.GetRequestId(),
            CorrelationId = request.GetCorrelationId(),
            HttpMethod = request.Method,
            RequestUrl = request.GetFullUrl(),
            Scheme = request.Scheme,
            Protocol = request.Protocol,
            IsHttps = request.IsHttps,
            QueryString = request.QueryString.ToString(),
            RouteValues = request.RouteValues.ToDictionary(k => k.Key, v => v.Value?.ToString()),
            UserAgent = request.GetUserAgent(),
            ClientId = request.GetClientId(),
            Headers = request.Headers.ToDictionary(k => k.Key, v => v.Value.ToList()),
            ContentType = request.GetContentType(),
            ContentLength = request.ContentLength ?? 0,
            Body = body,
            BodySize = body?.ToString()?.Length ?? 0,
            IsAjaxRequest = request.IsAjaxRequest(),
        };
    }

    /// <summary>
    /// Adds HTTP response information, including status, headers, and body.
    /// </summary>
    private ResponseInfoRequest AddResponseInfo()
    {
        var response = _context.Response;

        return new()
        {
            StatusCode = (HttpStatusCode)response.StatusCode,
            ReasonPhrase = ReasonPhrases.GetReasonPhrase(response.StatusCode),
            Headers = response.Headers.ToDictionary(k => k.Key, v => v.Value.ToList()),
            Body = GetCapturedResponseBody(),
            BodySize = response.ContentLength ?? 0
        };
    }

    /// <summary>
    /// Adds general diagnostic information about the execution environment.
    /// </summary>
    private DiagnosticRequest AddDiagnostics() => new()
    {
        MemoryUsageMb = GC.GetTotalMemory(false) / (1024.0 * 1024),
        DbQueryCount = 0,
        CacheHit = false,
        Dependencies = []
    };

    /// <summary>
    /// Adds log entries recorded during the current request.
    /// </summary>
    private IEnumerable<LogEntryRequest> AddLogEntries()
    {
        var entries = SnapTraceLogger.GetLogsForCurrentRequest(_context);

        return entries?.Select(e => new LogEntryRequest
        {
            LogCategory = e.Category,
            LogSeverity = e.LogLevel,
            LogMessage = e.Message,
            MemberType = e.MemberType,
            MemberName = e.MemberName,
            SourceLineNumber = e.LineNumber,
            Timestamp = e.DateTime
        }) ?? [];
    }

    /// <summary>
    /// Adds captured notifications to the log and updates severity level if needed.
    /// </summary>
    private IEnumerable<NotificationInfoRequest> AddNotifications()
    {
        if (_notifications?.Any() != true) return [];

        _logLevel = _notifications?.Any() == true
            ? _notifications.Max(n => n.LogLevel)
            : LogLevel.Information;

        if (_logLevel > LogLevel.Information)
            _errorCategory = "Notification";

        return _notifications.Select(n => new NotificationInfoRequest
        {
            Id = n.Id,
            Key = n.Key,
            Value = n.Value,
            LogLevel = n.LogLevel,
            Timestamp = n.Timestamp
        });
    }

    /// <summary>
    /// Adds captured exceptions to the log and sets severity level to critical.
    /// </summary>
    private IEnumerable<ExceptionInfoRequest> AddExceptions()
    {
        if (_exception == null) return [];

        _logLevel = LogLevel.Critical;
        _errorCategory = "Error";

        return
        [
            new ExceptionInfoRequest
            {
                Type = _exception.GetType().FullName,
                Message = _exception.Message,
                Tracer = _exception.StackTrace,
                Detail = _exception?.InnerException?.Message
            }
        ];
    }

    /// <summary>
    /// Retrieves and masks the HTTP request body, handling different content types.
    /// </summary>
    /// <returns>Object representing the request body, or null.</returns>
    private async Task<object?> GetRequestBodyAsync()
    {
        if (_context.Request.HasFormContentType)
        {
            var form = await _context.Request.ReadFormAsync();
            var dict = form.ToDictionary(x => x.Key, x => (object)x.Value.ToString());

            for (int i = 0; i < form.Files.Count; i++)
            {
                var file = form.Files[i];
                dict.Add($"{nameof(file.Name)}_{i}", file.Name);
                dict.Add($"{nameof(file.ContentType)}_{i}", file.ContentType);
                dict.Add($"{nameof(file.FileName)}_{i}", file.FileName);
                dict.Add($"{nameof(file.Length)}_{i}", file.Length);
            }

            return _sensitiveDataMasker.Mask(dict);
        }
        else
        {
            _context.Request.EnableBuffering();

            _context.Request.Body.Position = 0;
            using var reader = new StreamReader(_context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            _context.Request.Body.Position = 0;

            if (string.IsNullOrEmpty(body))
                return null;

            try
            {
                var obj = JsonExtensions.Deserialize<object>(body);
                return _sensitiveDataMasker.Mask(obj);
            }
            catch
            {
                return body;
            }
        }
    }

    /// <summary>
    /// Retrieves and masks the captured HTTP response body from the context.
    /// </summary>
    /// <returns>Masked JSON string or raw response body, or null.</returns>
    private string? GetCapturedResponseBody()
    {
        if (_context.Items.TryGetValue("CapturedResponseBody", out var bodyObj) && bodyObj is string body)
        {
            try
            {
                var obj = JsonExtensions.Deserialize<object>(body);
                var masked = _sensitiveDataMasker.Mask(obj);

                return JsonExtensions.Serialize(masked);
            }
            catch
            {
                return body;
            }
        }
        return null;
    }

    /// <summary>
    /// Retrieves the exception stored in the HTTP context, if any, and sets the log level.
    /// </summary>
    /// <returns>Captured exception or null.</returns>
    private Exception? GetException()
    {
        if (_context.Items.TryGetValue("Exception", out var exceptionObj) && exceptionObj is Exception exception)
        {
            _logLevel = LogLevel.Critical;
            return exception;
        }
        return null;
    }
}
