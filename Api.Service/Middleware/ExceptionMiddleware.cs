using Api.Responses;
using Common.Exceptions;
using Common.Extensions;
using Common.Json;
using Common.Logs.Extensions;
using Common.Notifications.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Api.Service.Middleware;

/// <summary>
/// Middleware responsible for handling exceptions in the request pipeline and returning a structured JSON response.
/// </summary>
public class ExceptionMiddleware
{
    public const string Name = "ExceptionMiddleware";
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance for logging exceptions.</param>
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Middleware execution logic for capturing and handling exceptions.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        Exception exception = null;

        try
        {
            context.Response.SetCorrelationId();

            await _next(context);
        }
        catch (DomainException ex)
        {
            await HandleRequestExceptionAsync(context, ex, HttpStatusCode.BadRequest);
        }
        catch (NotFoundException ex)
        {
            await HandleRequestExceptionAsync(context, ex, HttpStatusCode.NotFound);
        }
        catch (CustomHttpRequestException ex)
        {
            exception = ex;
            await HandleRequestExceptionAsync(context, ex, HttpStatusCode.BadGateway);
        }
        catch (UnauthorizedAccessException ex)
        {
            exception = ex;
            await HandleRequestExceptionAsync(context, ex, HttpStatusCode.Unauthorized);
        }
        catch (Exception ex)
        {
            exception = ex;
            await HandleRequestExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
        }
        finally
        {
            context.Items["Exception"] = exception;
        }
    }

    /// <summary>
    /// Handles specific exception types and maps them to corresponding HTTP status codes with appropriate logging.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="exception">The exception thrown.</param>
    /// <param name="statusCode">The HTTP status code to return.</param>
    private async Task HandleRequestExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
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
                return;
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
    }

    /// <summary>
    /// Serializes and sends the JSON response to the client based on the exception and log level.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="exception">The exception to include in the response.</param>
    /// <param name="statusCode">The HTTP status code to return.</param>
    /// <param name="logLevel">The log level used to log the exception.</param>
    private async Task SendExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode, LogLevel logLevel)
    {
        var notifications = new List<Notification>
        {
            new(logLevel, exception.GetType().Name, exception.GetType().Name, exception.Message)
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        string jsonResponse;

        if (logLevel == LogLevel.Warning)
        {
            jsonResponse = statusCode == HttpStatusCode.NotFound
                ? JsonExtensions.Serialize(new ApiResponse(new NotFoundResponse(exception.Message)) { CorrelationId = context.Request.GetCorrelationId() })
                : JsonExtensions.Serialize(new ApiResponse(new ValidationResponse(notifications)) { CorrelationId = context.Request.GetCorrelationId() });
        }
        else
        {
            jsonResponse = JsonExtensions.Serialize(new ApiResponse(statusCode, new ValidationResponse(notifications)) { CorrelationId = context.Request.GetCorrelationId() });
        }

        await context.Response.WriteAsync(jsonResponse);
    }
}
