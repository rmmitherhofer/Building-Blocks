using Api.Responses;
using Common.Extensions;
using Common.Notifications.Interfaces;
using Common.Notifications.Messages;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Api.Service.Controllers;

[ApiController]
[Produces("application/json")]
public abstract class MainController(INotificationHandler notification) : ControllerBase
{
    protected readonly INotificationHandler _notification = notification;

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
            return StatusCode((int)HttpStatusCode.InternalServerError,                
                new DetailsResponse(HttpStatusCode.InternalServerError, new ValidationResponse(_notification.Get().Where(v => v.LogLevel == LogLevel.Error)))
                {
                    CorrelationId = HttpContext.GetCorrelationId()
                });

        return BadRequest(new DetailsResponse(new ValidationResponse(_notification.Get()))
        {
            CorrelationId = HttpContext.GetCorrelationId()
        });
    }

    private ActionResult CustomResponse(ModelStateDictionary modelState)
    {
        foreach (var erro in modelState.Values.SelectMany(e => e.Errors))
        {
            Notify(nameof(ModelStateDictionary), null, erro.ErrorMessage);
        }
        return CustomResponse();
    }

    private ActionResult CustomResponse(ValidationResult validationResult)
    {
        foreach (var erro in validationResult.Errors)
        {
            Notify(nameof(ValidationResult), erro.PropertyName, erro.ErrorMessage);
        }
        return CustomResponse();
    }

    private ActionResult CustomResponse(NotFoundResponse response)
        => NotFound(new DetailsResponse(response)
        {
            CorrelationId = HttpContext.GetCorrelationId()
        });
    protected ActionResult NotFound(string? detail = null)
        => NotFound(new DetailsResponse(new NotFoundResponse(detail))
        {
            CorrelationId = HttpContext.GetCorrelationId()
        });
    protected bool IsValid()
        => !_notification.HasNotifications();

    protected void Notify(LogLevel logLevel, string type, string key, string value, string detail)
        => _notification.Notify(new Notification(logLevel, type, key, value, detail));
    protected void Notify(string type, string key, string value, string detail)
        => _notification.Notify(new Notification(LogLevel.Warning, type, key, value, detail));

    protected void Notify(LogLevel logLevel, string type, string key, string value)
        => _notification.Notify(new Notification(logLevel, type, key, value));
    protected void Notify(string type, string key, string value)
        => _notification.Notify(new Notification(LogLevel.Warning, type, key, value));

    protected void Notify(LogLevel logLevel, string key, string value)
        => _notification.Notify(new Notification(logLevel, key, value));
    protected void Notify(string key, string value)
        => _notification.Notify(new Notification(LogLevel.Warning, key, value));

    protected void Notify(LogLevel logLevel, string value)
        => _notification.Notify(new Notification(logLevel, value));
    protected void Notify(string value)
        => _notification.Notify(new Notification(LogLevel.Warning, value));

    protected void Notify(Exception exception)
        => _notification.Notify(new Notification(LogLevel.Error, exception.GetType().Name, exception.GetType().Name, exception.Message, exception?.StackTrace));
    protected void Clear()
        => _notification.Clear();
}