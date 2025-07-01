using Common.Extensions;
using Common.Notifications.Interfaces;
using Common.Notifications.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using SnapTrace.Adapters;
using SnapTrace.Configurations.Settings;
using SnapTrace.Enums;
using SnapTrace.Extensions;
using SnapTrace.HttpServices;
using SnapTrace.Models;
using System.Net;
using System.Text.Json;

namespace SnapTrace.Applications;

public class SnapTraceApplication : ISnapTraceApplication
{
    private readonly ISnapTraceHttpService _httpService;
    private readonly INotificationHandler _notification;
    private readonly SnapTraceSettings _settings;
    private HttpContext _context;

    public SnapTraceApplication(ISnapTraceHttpService httpService, IOptions<SnapTraceSettings> options, INotificationHandler notification)
    {
        _httpService = httpService;
        _settings = options.Value;
        _notification = notification;
    }

    public async Task Notify(HttpContext context, Exception exception, LogLevel logLevel, long elapsedMilliseconds)
    {
        if (!_settings.TurnOnLog) return;

        _context = context;

        var log = Create(logLevel, elapsedMilliseconds, nameof(Exception));

        log.Errors = AddException(exception);

        Task.Run(() => _httpService.Add(log));
    }
    public async Task Notify(HttpContext context, long elapsedMilliseconds)
    {
        if (!_settings.TurnOnLog) return;

        _context = context;

        var notifications = _notification.Get();

        LogLevel logLevel = notifications?.Any() == true
            ? notifications.Max(n => n.LogLevel)
            : LogLevel.Information;

        var log = Create(logLevel, elapsedMilliseconds, logLevel > LogLevel.Information ? nameof(Notification) : null);

        log.Errors = AddNotification(notifications.Where(x => x.LogLevel > LogLevel.Information));

        Task.Run(() => _httpService.Add(log));
    }

    private Log Create(LogLevel logLevel, long elapsedMilliseconds, string errorType)
    {
        var entries = SnapTraceLogger.GetLogsForCurrentRequest(_context);

        return new()
        {
            Path = _context.Request.Path,
            RequisitionTime = elapsedMilliseconds.GetTime(),
            LogLevel = logLevel,
            ErrorType = errorType,
            Entries = entries,
            Project = AddProject(),
            User = AddUser(),
            Request = AddRequest(),
            Response = AddResponse()
        };
    }
    private Project AddProject()
    {
        return new()
        {
            Name = _settings.Name,
            Type = _settings.ProjectType
        };
    }
    private User AddUser()
    {
        return new()
        {
            Id = _context.GetUserId(),
            HostName = Dns.GetHostName(),
            RemoteIP = _context.GetIpAddress(),
            ServerIP = _context?.Connection?.LocalIpAddress?.MapToIPv4()?.ToString(),
        };
    }
    private Request AddRequest()
    {
        string userAgent = _context.GetUserAgent();

        userAgent = string.IsNullOrEmpty(userAgent) ? "Unknown" : userAgent;

        return new()
        {
            Id = _context.GetRequestId(),
            Method = _context.Request.Method,
            Url = $"{_context.Request.Scheme}://{_context.Request.Host}{_context.Request.Path}{(_context.Request.QueryString.HasValue ? _context.Request.QueryString.Value : string.Empty)}",
            UserAgent = userAgent,
            Body = GetBody(),
            BodySize = _context.Request.ContentLength ?? 0,
            CorrelationId = _context.GetCorrelationId(),
            ClientId = _context.GetClientId(),
            Headers = Get(_context.Request.Headers),
        };
    }
    private Response AddResponse()
    {
        return new()
        {
            StatusCode = (HttpStatusCode)_context.Response.StatusCode,
            BodySize = _context.Response.ContentLength ?? 0,
            Headers = Get(_context.Response.Headers)
        };
    }
    private IEnumerable<Error> AddException(Exception exception)
    {
        if (exception is null) return [];

        List<Error> errors =
        [
            new()
            {
                Type = exception.GetType().FullName,
                Message = exception.Message,
                Tracer = exception.StackTrace,
                Datail = exception?.InnerException?.Message
            }
        ];

        return errors.Count > 0 ? errors : Enumerable.Empty<Error>();
    }
    private IEnumerable<Error> AddNotification(IEnumerable<Notification> notifications)
    {
        if (notifications?.Any() != true) return [];

        List<Error> errors = [];

        foreach (var notification in notifications)
        {
            errors.Add(new()
            {
                Type = nameof(Notification),
                LogLevel = notification.LogLevel.ToString(),
                Message = notification.Key,
                Tracer = notification.Value,
                Datail = notification.Detail
            });
        }

        return errors.Count > 0 ? errors : Enumerable.Empty<Error>();
    }
    private IDictionary<string, StringValues>? GetBody()
    {
        Dictionary<string, StringValues> result = [];

        try
        {
            if (_context.Request.HasFormContentType)
            {

                try
                {
                    foreach (var item in _context.Request.Form)
                        result.Add(item.Key, item.Value);
                }
                catch { }

                try
                {
                    var i = 0;

                    foreach (var file in _context.Request.Form.Files)
                    {
                        result.Add($"{nameof(file.Name)}_{i}", file.Name);
                        result.Add($"{nameof(file.ContentType)}_{i}", file.ContentType);
                        result.Add($"{nameof(file.FileName)}_{i}", file.FileName);
                        result.Add($"{nameof(file.Length)}_{i}", file.Length.ToString());
                    }
                }
                catch { }
            }
            else
            {
                string body = _context.Request.GetBody();

                if (!string.IsNullOrEmpty(body))
                    foreach (var item in JsonSerializer.Deserialize<Dictionary<string, object>>(body))
                        result.Add(item.Key, item.Value?.ToString());

            }

            return result;
        }
        catch
        {
            return default;
        }
    }
    private IDictionary<string, StringValues>? Get(IHeaderDictionary headers)
    {
        Dictionary<string, StringValues> result = [];

        if (!headers.Any()) return default;

        foreach (var item in headers)
            result.Add(item.Key, item.Value);

        return result;
    }
}
