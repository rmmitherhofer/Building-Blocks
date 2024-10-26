using Common.Notifications.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SnapTrace.Applications;

public interface ISnapTraceApplication
{
    Task Notify(HttpContext context, Exception exception, LogLevel logLevel, long elapsedMilliseconds);
    Task Notify(HttpContext context, IEnumerable<Notification> notifications, LogLevel logLevel, long elapsedMilliseconds);
}
