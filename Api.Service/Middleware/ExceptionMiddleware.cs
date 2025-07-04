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
            logLevel = await HandleRequestExceptionAsync(context, ex, HttpStatusCode.BadRequest);
        }
        catch (NotFoundException ex)
        {
            logLevel = await HandleRequestExceptionAsync(context, ex, HttpStatusCode.NotFound);
        }
        catch (CustomHttpRequestException ex)
        {
            exception = ex;
            logLevel = await HandleRequestExceptionAsync(context, ex, HttpStatusCode.BadGateway);
        }
        catch (UnauthorizedAccessException ex)
        {
            exception = ex;
            logLevel = await HandleRequestExceptionAsync(context, ex, HttpStatusCode.Unauthorized);
        }
        catch (Exception ex)
        {
            exception = ex;
            logLevel = await HandleRequestExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
        }
        finally
        {
            context.Items["Exception"] = exception;
        }
    }

    private async Task<LogLevel> HandleRequestExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
    {
        LogLevel logLevel = LogLevel.Information;
        switch (statusCode)
        {
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.BadRequest:
            case HttpStatusCode.NotFound:
                logLevel = LogLevel.Warning;
                _logger.LogWarn($"Message: {exception.Message}");
                await SendExceptionAsync(context, exception, statusCode, logLevel);
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

        await SendExceptionAsync(context, exception, statusCode, logLevel);

        return logLevel;
    }

    private async Task SendExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode, LogLevel logLevel)
    {
        var notifications = new List<Notification> { new(logLevel, exception.GetType().Name, exception.GetType().Name, exception.Message, logLevel == LogLevel.Critical ? exception?.StackTrace! : null) };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        string jsonResponse;

        if (logLevel == LogLevel.Warning)
        {
            jsonResponse = statusCode == HttpStatusCode.NotFound
                ? JsonConvert.SerializeObject(new DetailsResponse(new NotFoundResponse(exception.Message)) { CorrelationId = context.GetCorrelationId() })
                : JsonConvert.SerializeObject(new DetailsResponse(new ValidationResponse(notifications)) { CorrelationId = context.GetCorrelationId() });
        }
        else
        {
            jsonResponse = JsonConvert.SerializeObject(new DetailsResponse(statusCode, new ValidationResponse(notifications)) { CorrelationId = context.GetCorrelationId() });
        }

        await context.Response.WriteAsync(jsonResponse);
    }
}
