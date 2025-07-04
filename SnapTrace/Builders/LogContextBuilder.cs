using Common.Extensions;
using Common.Notifications.Interfaces;
using Common.Notifications.Messages;
using Extensoes;
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
using System.Text.Json;

namespace SnapTrace.Builders;

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

    public LogContextBuilder(IOptions<SnapTraceSettings> options, SensitiveDataMasker sensitiveDataMasker, INotificationHandler notification)
    {
        _settings = options.Value;
        _sensitiveDataMasker = sensitiveDataMasker;
        _notification = notification;
    }

    public ILogContextBuilder WithHttpContext(HttpContext context)
    {
        _context = context;
        return this;
    }

    public ILogContextBuilder WithElapsedMilliseconds(long elapsedMs)
    {
        _elapsedMs = elapsedMs;
        return this;
    }

    public ILogContextBuilder WithNotifications()
    {
        _notifications = _notification.Get() ?? [];
        return this;
    }

    public ILogContextBuilder WithException()
    {
        _exception = GetException();
        return this;
    }

    public async Task<LogContextRequest> BuildAsync()
    {
        return new LogContextRequest
        {
            EndpointPath = _context.Request.Path,
            RequestDuration = _elapsedMs.GetTime(),
            ElapsedMilliseconds = _elapsedMs,
            LogSeverity = (int)_logLevel,
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
            Exceptions = AddExceptions()
        };
    }

    private ProjectInfo AddProject() => new()
    {
        Id = _settings.ProjectId,
        Name = _settings.Name,
        Type = _settings.ProjectType
    };

    private EnvironmentInfo AddEnvironment() => new()
    {
        MachineName = Environment.MachineName,
        EnvironmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
        ApplicationVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "Unknown",
        ThreadId = Environment.CurrentManagedThreadId
    };
    private UserContextRequest AddUserContext()
    {
        var user = _context.User;

        return new()
        {
            Id = _context.GetUserId(),
            Code = null,
            Name = _context.GetUserName(),
            IsAuthenticated = _context.IsAuthenticated(),
            AuthenticationType = _context.User.Identity?.AuthenticationType,
            Roles = user.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList(),
            Claims = user.Claims.ToDictionary(c => c.Type, c => c.Value),
            IpAddress = _context.GetIpAddress()
        };
    }
    private async Task<RequestInfoRequest> AddRequestInfoAsync()
    {
        var req = _context.Request;

        var body = await GetRequestBodyAsync();

        return new()
        {
            RequestId = _context.GetRequestId(),
            CorrelationId = _context.GetCorrelationId(),
            HttpMethod = req.Method,
            RequestUrl = $"{req.Scheme}://{req.Host}{req.Path}{(req.QueryString.HasValue ? req.QueryString.Value : string.Empty)}",
            Scheme = req.Scheme,
            Protocol = req.Protocol,
            IsHttps = req.IsHttps,
            QueryString = req.QueryString.ToString(),
            RouteValues = req.RouteValues.ToDictionary(k => k.Key, v => v.Value?.ToString()),
            UserAgent = _context.GetUserAgent(),
            ClientId = _context.GetClientId(),
            Headers = req.Headers.ToDictionary(k => k.Key, v => v.Value.ToList()),
            ContentType = req.ContentType,
            ContentLength = req.ContentLength ?? 0,
            Body = body,
            BodySize = body?.ToString()?.Length ?? 0
        };
    }
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

    private DiagnosticRequest AddDiagnostics() => new()
    {
        MemoryUsageMb = GC.GetTotalMemory(false) / (1024.0 * 1024),
        DbQueryCount = 0,
        CacheHit = false,
        Dependencies = []
    };

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
                var obj = JsonSerializer.Deserialize<object>(body);
                return _sensitiveDataMasker.Mask(obj);
            }
            catch
            {
                return body;
            }
        }
    }

    private string? GetCapturedResponseBody()
    {
        if (_context.Items.TryGetValue("CapturedResponseBody", out var bodyObj) && bodyObj is string body)
        {
            try
            {
                var jsonDoc = JsonDocument.Parse(body);
                var obj = JsonSerializer.Deserialize<object>(body);
                var masked = _sensitiveDataMasker.Mask(obj);

                return JsonSerializer.Serialize(masked);
            }
            catch
            {
                return body;
            }
        }
        return null;
    }

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