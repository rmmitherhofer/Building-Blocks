using Api.Responses;
using Common.Exceptions;
using Common.Extensions;
using Common.Notifications.Messages;
using Logs.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace Api.Service.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    //private readonly ISnapTraceApplication _snapTrace;
    //private Stopwatch _diagnostic;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger/*, ISnapTraceApplication snapTrace*/)
    {
        //ArgumentNullException.ThrowIfNull(snapTrace, nameof(ISnapTraceApplication));

        _next = next;
        _logger = logger;
        //_snapTrace = snapTrace;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Exception exception = null;
        LogLevel logLevel = LogLevel.Trace;
        //_diagnostic = new();
        //_diagnostic.Start();

        try
        {
            context.Response.SetCorrelationId();

            await _next(context);


        }
        catch (DomainException ex)
        {
            exception = ex;
            logLevel = HandleRequestException(context, ex, HttpStatusCode.BadRequest);
        }
        catch (CustomHttpRequestException ex)
        {
            exception = ex;
            logLevel = HandleRequestException(context, ex, HttpStatusCode.BadGateway);
        }
        catch (UnauthorizedAccessException ex)
        {
            exception = ex;
            logLevel = HandleRequestException(context, ex, HttpStatusCode.Unauthorized);
        }
        catch (Exception ex)
        {
            exception = ex;
            logLevel = HandleRequestException(context, ex, HttpStatusCode.InternalServerError);
        }
        finally
        {
            if (exception is not null)
                throw exception;
        }
    }

    private LogLevel HandleRequestException(HttpContext context, Exception exception, HttpStatusCode statusCode)
    {
        LogLevel logLevel = LogLevel.Information;
        switch (statusCode)
        {
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.BadRequest:
                logLevel = LogLevel.Warning;
                _logger.LogWarn($"Message: {exception.Message} - detail: {exception.StackTrace}");
                SendException(context, exception, statusCode, logLevel);
                return logLevel;
            case HttpStatusCode.BadGateway:
                logLevel = LogLevel.Error;
                _logger.LogFail($"Message: {exception.Message} - detail: {exception.StackTrace}");
                break;
            case HttpStatusCode.InternalServerError:
                logLevel = LogLevel.Critical;
                _logger.LogCrit($"Message: {exception.Message} - detail: {exception.StackTrace}");
                break;
        }
        SendException(context, exception, statusCode, logLevel);

        return logLevel;
    }
    private void SendException(HttpContext context, Exception exception, HttpStatusCode statusCode, LogLevel logLevel)
    {
        var notifications = new List<Notification> { new(logLevel, exception.GetType().Name, exception.GetType().Name, exception.Message, logLevel == LogLevel.Critical ? exception?.StackTrace! : null) };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        switch (logLevel)
        {
            case LogLevel.Warning:
                context.Response.WriteAsync(JsonConvert.SerializeObject(new DetailsResponse(new ValidationResponse(notifications))
                {
                    CorrelationId = context.GetCorrelationId()
                }));
                break;
            default:
                context.Response.WriteAsync(JsonConvert.SerializeObject(new DetailsResponse(statusCode, new ValidationResponse(notifications))
                {
                    CorrelationId = context.GetCorrelationId()
                }));
                break;
        }
    }
}
