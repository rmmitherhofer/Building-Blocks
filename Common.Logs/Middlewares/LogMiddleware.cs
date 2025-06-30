using Common.Extensions;
using Logs.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Logs.Middlewares;

internal class LogMiddleware(ILogger<LogMiddleware> logger, RequestDelegate next)
{
    private readonly ILogger<LogMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;
    private Stopwatch _diagnostic;

    public async Task InvokeAsync(HttpContext context)
    {
        _diagnostic = new();
        _diagnostic.Start();

        HeaderRequest(context);

        _logger.LogInfo($"Request: {context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty)}");

        await _next(context);

        _diagnostic.Stop();

        _logger.LogInfo($"Response: {context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty)} - Timer request: {_diagnostic.ElapsedMilliseconds.GetTime()} - Status code: {context.Response.StatusCode} - {(HttpStatusCode)context.Response.StatusCode}");
    }

    private void HeaderRequest(HttpContext context)
    {
        StringBuilder builder = new();

        builder.AppendLine("====================================================================================================");
        builder.AppendLine($"Date/Hour: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        builder.AppendLine($"Method: {context.Request.Method}");
        builder.AppendLine($"Url: {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty)}");
        builder.AppendLine($"Request-ID: {context.GetRequestId()}");
        builder.AppendLine($"Correlation-ID: {context.GetCorrelationId()}");

        if(!string.IsNullOrEmpty(context.GetClientId()))
            builder.AppendLine($"Client-ID: {context.GetClientId()}");

        if(!string.IsNullOrEmpty(context.GetUserAgent()))
            builder.AppendLine($"User-Agent: {context.GetUserAgent()}");

        if(!string.IsNullOrEmpty(context.GetIpAddress()))
            builder.AppendLine($"IP-Address {context.GetIpAddress()}");

        if(!string.IsNullOrEmpty(context.GetUserId()))
            builder.AppendLine($"User-ID: {context.GetUserId()}");

        if(!string.IsNullOrEmpty(context.GetPodName()))
            builder.AppendLine($"Pod-Request {context.GetPodName()}");

        builder.AppendLine("====================================================================================================");

        Console.WriteLine(builder.ToString());
    }
}
