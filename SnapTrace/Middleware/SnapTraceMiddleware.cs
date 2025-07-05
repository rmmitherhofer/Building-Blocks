using Microsoft.AspNetCore.Http;
using SnapTrace.Applications;
using System.Diagnostics;

namespace SnapTrace.Middleware;

/// <summary>
/// Middleware that measures request execution time and triggers SnapTrace notification
/// after the request is completed, regardless of success or exception.
/// </summary>
public class SnapTraceMiddleware
{
    /// <summary>
    /// Middleware name identifier.
    /// </summary>
    public const string Name = "SnapTraceMiddleware";

    private readonly RequestDelegate _next;
    private readonly ISnapTraceApplication _snapTrace;
    private Stopwatch _diagnostic;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnapTraceMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="snapTrace">The SnapTrace application service used to send log data.</param>
    public SnapTraceMiddleware(RequestDelegate next, ISnapTraceApplication snapTrace)
    {
        ArgumentNullException.ThrowIfNull(snapTrace, nameof(ISnapTraceApplication));

        _next = next;
        _snapTrace = snapTrace;
    }

    /// <summary>
    /// Intercepts the HTTP request, measures its execution time, and sends data to SnapTrace.
    /// In case of an exception, the exception is stored in the HTTP context for later processing.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
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
            context.Items["Exception"] = ex;
            throw;
        }
        finally
        {
            if (!context.Request.Path.Value.Contains("swagger"))
                _ = _snapTrace.Notify(context, _diagnostic.ElapsedMilliseconds);
        }
    }
}
