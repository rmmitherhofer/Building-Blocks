using Api.Responses;
using Common.Exceptions;
using Common.Notifications.Messages;
using Logs.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SnapTrace.Applications;
using System.Diagnostics;
using System.Net;

namespace Api.Service.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly ISnapTraceApplication _snapTrace;
    private Stopwatch _diagnostic;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger , ISnapTraceApplication snapTrace)
    {
        ArgumentNullException.ThrowIfNull(snapTrace, nameof(ISnapTraceApplication));

        _next = next;
        _logger = logger;
        _snapTrace = snapTrace;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _diagnostic = new();
        _diagnostic.Start();

        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            HandleRequestException(context, ex, HttpStatusCode.BadRequest);
        }
        catch (CustomHttpRequestException ex)
        {
            HandleRequestException(context, ex, HttpStatusCode.BadGateway);
        }
        catch (UnauthorizedAccessException ex)
        {
            HandleRequestException(context, ex, HttpStatusCode.Unauthorized);
        }
        catch (Exception ex)
        {
            HandleRequestException(context, ex, HttpStatusCode.InternalServerError);
        }
        finally { _diagnostic.Stop(); }
    }

    private void HandleRequestException(HttpContext context, Exception exception, HttpStatusCode statusCode)
    {
        LogLevel logLevel = LogLevel.Information;
        switch (statusCode)
        {
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.BadRequest:
                logLevel = LogLevel.Warning;
                _logger.LogWarn($"Message: {exception.Message} - detail: {exception.StackTrace}", context);
                SendException(context, exception, statusCode, logLevel);
                return;
            case HttpStatusCode.BadGateway:
                logLevel = LogLevel.Error;
                _logger.LogFail($"Message: {exception.Message} - detail: {exception.StackTrace}", context);
                break;
            case HttpStatusCode.InternalServerError:
                logLevel = LogLevel.Critical;
                _logger.LogCrit($"Message: {exception.Message} - detail: {exception.StackTrace}", context);
                break;
        }
        SendException(context, exception, statusCode, logLevel);
    }
    private void SendException(HttpContext context, Exception exception, HttpStatusCode statusCode, LogLevel logLevel)
    {
        _snapTrace.Notify(context, exception, logLevel, _diagnostic.ElapsedMilliseconds);

        var notifications = new List<Notification> { new(logLevel, exception.GetType().Name, exception.GetType().Name, exception.Message, logLevel == LogLevel.Critical ? exception?.StackTrace! : null) };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        switch (logLevel)
        {
            case LogLevel.Warning:
                context.Response.WriteAsync(JsonConvert.SerializeObject(new ValidationResponse(notifications)));
                break;
            default:
                context.Response.WriteAsync(JsonConvert.SerializeObject(new ErrorResponse(statusCode, notifications)));
                break;
        }
    }
}