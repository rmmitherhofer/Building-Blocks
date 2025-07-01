using Api.Responses;
using Common.Exceptions;
using Common.Extensions;
using Common.Notifications.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace Api.Service.Middleware;

public class ExceptionMiddleware
{
    public const string Name = "ExceptionMiddleware";
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Exception exception = null;
        LogLevel logLevel = LogLevel.Trace;

        try
        {
            context.Response.SetCorrelationId();

            await _next(context);
        }
        catch (DomainException ex)
        {
            logLevel = HandleRequestException(context, ex, HttpStatusCode.BadRequest);
        }
        catch (NotFoundException ex)
        {
            logLevel = HandleRequestException(context, ex, HttpStatusCode.NotFound);
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
            case HttpStatusCode.NotFound:
                logLevel = LogLevel.Warning;
                _logger.LogWarn($"Message: {exception.Message}");
                SendException(context, exception, statusCode, logLevel);
                return logLevel;
            case HttpStatusCode.BadGateway:
                logLevel = LogLevel.Error;
                _logger.LogFail($"Message: {exception.Message}");
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
                switch (statusCode)
                {
                    case HttpStatusCode.NotFound:
                        context.Response.WriteAsync(JsonConvert.SerializeObject(new DetailsResponse(new NotFoundResponse(exception.Message))
                        {
                            CorrelationId = context.GetCorrelationId()
                        }));
                        break;
                    default:
                        context.Response.WriteAsync(JsonConvert.SerializeObject(new DetailsResponse(new ValidationResponse(notifications))
                        {
                            CorrelationId = context.GetCorrelationId()
                        }));
                        break;
                }
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
