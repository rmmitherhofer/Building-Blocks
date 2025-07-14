using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Net;
using Zypher.Extensions.Core;
using Zypher.Notifications.Interfaces;
using Zypher.Notifications.Messages;
using Zypher.Responses;
using Zypher.Responses.Factories;

namespace Zypher.Api.Foundation.Controllers;

/// <summary>
/// Base controller class that provides consistent response handling and notification integration.
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class MainController(INotificationHandler notification) : ControllerBase
{
    protected readonly INotificationHandler _notification = notification;

    /// <summary>
    /// Returns a custom API response based on the current notification state and input result.
    /// </summary>
    protected ActionResult CustomResponse(object? result = null)
    {
        if (result is not null)
        {
            switch (result.GetType().Name)
            {
                case nameof(NotFoundResponse):
                    return CustomResponse(result as NotFoundResponse);
                case nameof(ModelStateDictionary):
                    return CustomResponse(result as ModelStateDictionary);
                case nameof(ValidationResult):
                    return CustomResponse(result as ValidationResult);
            }
        }
        if (IsValid())
            return Ok(result);

        if (_notification.Get().Select(v => v.LogLevel).Contains(LogLevel.Error))
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                new ApiResponse(HttpStatusCode.InternalServerError, new ValidationResponse(_notification.Get().Where(v => v.LogLevel == LogLevel.Error).ToResponse()))
                {
                    CorrelationId = HttpContext.Request.GetCorrelationId()
                });
        }

        return BadRequest(new ApiResponse(new ValidationResponse(_notification.Get().ToResponse()))
        {
            CorrelationId = HttpContext.Request.GetCorrelationId()
        });;
    }

    /// <summary>
    /// Processes validation errors from model state and generates a response.
    /// </summary>
    private ActionResult CustomResponse(ModelStateDictionary modelState)
    {
        foreach (var erro in modelState.Values.SelectMany(e => e.Errors))
        {
            Notify(nameof(ModelStateDictionary), null, erro.ErrorMessage);
        }
        return CustomResponse();
    }

    /// <summary>
    /// Processes validation errors from FluentValidation and generates a response.
    /// </summary>
    private ActionResult CustomResponse(ValidationResult validationResult)
    {
        foreach (var erro in validationResult.Errors)
        {
            Notify(nameof(ValidationResult), erro.PropertyName, erro.ErrorMessage);
        }
        return CustomResponse();
    }

    /// <summary>
    /// Returns a NotFound response with a NotFoundResponse payload.
    /// </summary>
    private ActionResult CustomResponse(NotFoundResponse response)
        => NotFound(new ApiResponse(response)
        {
            CorrelationId = HttpContext.Request.GetCorrelationId()
        });

    /// <summary>
    /// Returns a NotFound response with an optional detail message.
    /// </summary>
    protected ActionResult NotFound(string? detail = null)
        => NotFound(new ApiResponse(new NotFoundResponse(detail))
        {
            CorrelationId = HttpContext.Request.GetCorrelationId()
        });

    /// <summary>
    /// Checks if there are no notifications present.
    /// </summary>
    protected bool IsValid()
        => !_notification.HasNotifications();

    /// <summary>
    /// Adds a notification with full details.
    /// </summary>
    protected void Notify(LogLevel logLevel, string type, string key, string value, string detail)
        => _notification.Notify(new Notification(logLevel, type, key, value, detail));

    /// <summary>
    /// Adds a warning notification with full details.
    /// </summary>
    protected void Notify(string type, string key, string value, string detail)
        => _notification.Notify(new Notification(LogLevel.Warning, type, key, value, detail));

    /// <summary>
    /// Adds a notification with the given log level and message parts.
    /// </summary>
    protected void Notify(LogLevel logLevel, string type, string key, string value)
        => _notification.Notify(new Notification(logLevel, type, key, value));

    /// <summary>
    /// Adds a warning notification with type, key, and value.
    /// </summary>
    protected void Notify(string type, string key, string value)
        => _notification.Notify(new Notification(LogLevel.Warning, type, key, value));

    /// <summary>
    /// Adds a notification with the given log level and message.
    /// </summary>
    protected void Notify(LogLevel logLevel, string key, string value)
        => _notification.Notify(new Notification(logLevel, key, value));

    /// <summary>
    /// Adds a warning notification with key and value.
    /// </summary>
    protected void Notify(string key, string value)
        => _notification.Notify(new Notification(LogLevel.Warning, key, value));

    /// <summary>
    /// Adds a notification with the given log level and message.
    /// </summary>
    protected void Notify(LogLevel logLevel, string value)
        => _notification.Notify(new Notification(logLevel, value));

    /// <summary>
    /// Adds a warning notification with the given message.
    /// </summary>
    protected void Notify(string value)
        => _notification.Notify(new Notification(LogLevel.Warning, value));

    /// <summary>
    /// Adds an error notification based on an exception.
    /// </summary>
    protected void Notify(Exception exception)
        => _notification.Notify(new Notification(LogLevel.Error, exception.GetType().Name, exception.GetType().Name, exception.Message, exception?.StackTrace));

    /// <summary>
    /// Clears all current notifications.
    /// </summary>
    protected void Clear()
        => _notification.Clear();
}
