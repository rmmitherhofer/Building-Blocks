using Common.Extensions;
using Logs.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;

namespace Logs.Middlewares;

public class LogMiddleware(ILogger<LogMiddleware> logger, RequestDelegate next)
{
    private readonly ILogger<LogMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;
    private Stopwatch _diagnostic;

    public async Task InvokeAsync(HttpContext context)
    {
        _diagnostic = new();
        _diagnostic.Start();

        SetUserClaims(context);

        _logger.LogInfo($"{(!string.IsNullOrEmpty(context.GetUserId()) ? $"User ID: {context.GetUserId()} | " : string.Empty)}Request: {context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty)}", context);

        await _next(context);

        _diagnostic.Stop();

        _logger.LogInfo($"{(!string.IsNullOrEmpty(context.GetUserId()) ? $"User ID: {context.GetUserId()} | " : string.Empty)}Response: {context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty)} - Timer request: {_diagnostic.ElapsedMilliseconds.GetTime()} - Status code: {context.Response.StatusCode} - {(HttpStatusCode)context.Response.StatusCode}", context);
    }

    private void SetUserClaims(HttpContext context)
    {
        context.AddClaimUserCode(context.Request.Headers[HttpContextExtensions.USER_CODE]);

        context.AddClaimUserId(context.Request.Headers[HttpContextExtensions.USER_ID]);

        context.AddClaimSessionId(context.Request.Headers[HttpContextExtensions.USER_SESSION_ID]);

        if (string.IsNullOrEmpty(context.GetSessionId()))
            context.AddClaimSessionId();

        context.AddClaimIpAddress(context.Request.Headers[HttpContextExtensions.USER_IP_ADDRESS]);

        if (string.IsNullOrEmpty(context.GetIpAddress()))
            context.AddClaimIpAddress(context.GetIpAddressForwarded());
    }
}
