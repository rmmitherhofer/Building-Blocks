using Microsoft.AspNetCore.Http;
using Common.Extensions;
using Microsoft.Extensions.Logging;
using Logs.Extensions;
using System.Diagnostics;
using System.Net;

namespace SnapTraceV2.Middlewares;

public class SnapTraceMiddleware
{
    private readonly ILogger<SnapTraceMiddleware> _logger;
    private readonly RequestDelegate _next;
    private Stopwatch _diagnostic;

    public SnapTraceMiddleware(RequestDelegate next, ILogger<SnapTraceMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    public SnapTraceMiddleware(ILogger<SnapTraceMiddleware> logger)
    {

    }
    public async Task InvokeAsync(HttpContext context)
    {
        _diagnostic = new();
        _diagnostic.Start();

        try
        {
            SetUserClaims(context);

            _logger.LogInfo($"{(!string.IsNullOrEmpty(context.GetUserId()) ? $"User ID: {context.GetUserId()} | " : string.Empty)}Request: {context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty)}", context);

            await _next(context);
        }
        catch (Exception e)
        {
            throw;
        }
        finally
        {
            _diagnostic.Stop();

            _logger.LogInfo($"{(!string.IsNullOrEmpty(context.GetUserId()) ? $"User ID: {context.GetUserId()} | " : string.Empty)}Response: {context.Request.Method} {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty)} - Timer request: {_diagnostic.ElapsedMilliseconds.GetTime()} - Status code: {context.Response.StatusCode} - {(HttpStatusCode)context.Response.StatusCode}", context);
        }
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
