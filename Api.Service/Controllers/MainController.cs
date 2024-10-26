using Api.Responses;
using Common.Notifications.Interfaces;
using Common.Notifications.Messages;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
            return Problem(detail: JsonConvert.SerializeObject(new ErrorResponse(_notification.Get().Where(v => v.LogLevel == LogLevel.Error))));

        return BadRequest(new ValidationResponse(_notification.Get()));
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
        => NotFound(response);
    protected ActionResult NotFound(string? detail = null)
        => NotFound(new NotFoundResponse(detail));
    protected bool IsValid()
        => !_notification.HasNotifications();
    protected void Notify(string type, string key, string mensagem)
        => _notification.Notify(new Notification(type, key, mensagem));
    protected void Notify(string key, string mensagem)
        => _notification.Notify(new Notification(key, mensagem));
    protected void Notify(Exception exception)
        => _notification.Notify(new Notification(LogLevel.Error, exception.GetType().Name, exception.GetType().Name, exception.Message, exception?.StackTrace));
    protected void Clear()
        => _notification.Clear();
}