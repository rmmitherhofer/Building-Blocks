using Api.Responses;
using Common.Exceptions;
using Common.Notifications.Messages;
using Logs.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace Api.Service.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private Stopwatch _diagnostico;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _diagnostico = new();
        _diagnostico.Start();

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
        finally { _diagnostico.Stop(); }
    }

    private void HandleRequestException(HttpContext context, Exception exception, HttpStatusCode statusCode)
    {
        LogLevel levelLog = LogLevel.Information;
        switch (statusCode)
        {
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.BadRequest:
                levelLog = LogLevel.Warning;
                _logger.LogWarn($"Descricao: {exception.Message} - detalhe: {exception.StackTrace}");
                SendException(context, exception, statusCode, levelLog);
                return;
            case HttpStatusCode.BadGateway:
                levelLog = LogLevel.Error;
                _logger.LogFail($"Descricao: {exception.Message} - detalhe: {exception.StackTrace}");
                break;
            case HttpStatusCode.InternalServerError:
                levelLog = LogLevel.Critical;
                _logger.LogCrit($"Descricao: {exception.Message} - detalhe: {exception.StackTrace}");
                break;
        }
        SendException(context, exception, statusCode, levelLog);
    }
    private void SendException(HttpContext context, Exception exception, HttpStatusCode statusCode, LogLevel levelLog)
    {
        var notifications = new List<Notification> { new(levelLog, exception.GetType().Name, exception.GetType().Name, exception.Message, levelLog == LogLevel.Critical ? exception?.StackTrace! : null) };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        switch (levelLog)
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
