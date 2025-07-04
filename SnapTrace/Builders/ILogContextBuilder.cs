using Common.Notifications.Messages;
using Microsoft.AspNetCore.Http;
using SnapTrace.Models;

namespace SnapTrace.Builders;

public interface ILogContextBuilder
{
    ILogContextBuilder WithHttpContext(HttpContext context);
    ILogContextBuilder WithElapsedMilliseconds(long elapsedMs);
    ILogContextBuilder WithNotifications();
    ILogContextBuilder WithException();
    Task<LogContextRequest> BuildAsync();
}
