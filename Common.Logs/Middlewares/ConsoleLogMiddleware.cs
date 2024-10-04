using Logs.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;

namespace Logs.Middlewares;

public class ConsoleLogMiddleware
{
    private readonly ILogger<ConsoleLogMiddleware> _logger;
    private readonly RequestDelegate _next;
    private Stopwatch? _diagnostic;

    public ConsoleLogMiddleware(ILogger<ConsoleLogMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _diagnostic = new();
        _diagnostic.Start();

        _logger.LogInfo($"Request: {context.Request.Method}-{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty)}");

        await _next(context);

        _diagnostic.Stop();

        _logger.LogInfo(@$"Response: {context.Request.Method}-{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty)}
                        Time-Request {ExecutionTime()} - StatusCode: {context.Response.StatusCode} - {(HttpStatusCode)context.Response.StatusCode}");
    }

    private string ExecutionTime()
    {
        var time = TimeSpan.FromMilliseconds(_diagnostic.ElapsedMilliseconds);
        var timeOnly = TimeOnly.FromTimeSpan(time);

        return timeOnly.ToString("HH:mm:ss.fff");
    }
}
