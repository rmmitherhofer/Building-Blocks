using Common.Notifications.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SnapTrace.Applications;
using System.Diagnostics;

namespace SnapTrace.Middleware;

public class SnapTraceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SnapTraceMiddleware> _logger;
    private readonly ISnapTraceApplication _snapTrace;
    private Stopwatch _diagnostic;

    public SnapTraceMiddleware(RequestDelegate next, ILogger<SnapTraceMiddleware> logger, ISnapTraceApplication snapTrace)
    {
        ArgumentNullException.ThrowIfNull(snapTrace, nameof(ISnapTraceApplication));

        _next = next;
        _logger = logger;
        _snapTrace = snapTrace;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Exception exception = null;
        try
        {
            _diagnostic = new();
            _diagnostic.Start();

            await _next(context);

            _diagnostic.Stop();
        }
        catch (Exception ex)
        {
            exception = ex;
        }
        finally
        {
            if (exception is null)
                _ = _snapTrace.Notify(context, _diagnostic.ElapsedMilliseconds);
            else
                _ = _snapTrace.Notify(context, exception, LogLevel.Error, _diagnostic.ElapsedMilliseconds);
        }
    }
}