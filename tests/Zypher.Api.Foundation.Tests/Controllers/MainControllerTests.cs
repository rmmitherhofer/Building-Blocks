using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;
using Zypher.Api.Foundation.Controllers;
using Zypher.Notifications.Interfaces;
using Zypher.Notifications.Messages;
using Zypher.Responses;

namespace Zypher.Api.Foundation.Tests.Controllers;

public class MainControllerTests
{
    private sealed class FakeNotificationHandler : INotificationHandler
    {
        private readonly List<Notification> _items = new();

        public void Notify(Notification notification) => _items.Add(notification);
        public void Notify(IEnumerable<Notification> notifications) => _items.AddRange(notifications);
        public IEnumerable<Notification> Get() => _items;
        public bool HasNotifications() => _items.Any(x => x.LogLevel > Microsoft.Extensions.Logging.LogLevel.Information);
        public void Clear() => _items.Clear();
    }

    private sealed class TestController : MainController
    {
        public TestController(INotificationHandler notification) : base(notification)
        {
        }

        public ActionResult CallCustomResponse(object? result = null) => CustomResponse(result);
        public ActionResult CallNotFound(string? detail = null) => NotFound(detail);
        public void CallNotify(string value) => Notify(value);
        public void CallNotifyError(string key, string value) => Notify(Microsoft.Extensions.Logging.LogLevel.Error, key, value);
        public void CallNotifyWarning(string key, string value) => Notify(key, value);
        public void CallNotify(Exception exception) => Notify(exception);
        public void CallClear() => Clear();
    }

    private static TestController CreateController(FakeNotificationHandler handler)
    {
        var controller = new TestController(handler);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    [Fact(DisplayName =
        "Given no notifications, " +
        "When CustomResponse is called, " +
        "Then it returns Ok")]
    [Trait("Type", nameof(MainController))]
    public async Task CustomResponse_NoNotifications_ReturnsOk()
    {
        //Given
        var handler = new FakeNotificationHandler();
        var controller = CreateController(handler);

        //When
        var result = controller.CallCustomResponse(new { ok = true });

        //Then
        result.Should().BeOfType<OkObjectResult>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given warning notifications, " +
        "When CustomResponse is called, " +
        "Then it returns BadRequest with ApiResponse")]
    [Trait("Type", nameof(MainController))]
    public async Task CustomResponse_Warnings_ReturnsBadRequest()
    {
        //Given
        var handler = new FakeNotificationHandler();
        var controller = CreateController(handler);
        controller.CallNotifyWarning("key", "value");

        //When
        var result = controller.CallCustomResponse();

        //Then
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeOfType<ApiResponse>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given error notifications, " +
        "When CustomResponse is called, " +
        "Then it returns StatusCode 500")]
    [Trait("Type", nameof(MainController))]
    public async Task CustomResponse_Errors_ReturnsInternalServerError()
    {
        //Given
        var handler = new FakeNotificationHandler();
        var controller = CreateController(handler);
        controller.CallNotifyError("key", "value");

        //When
        var result = controller.CallCustomResponse();

        //Then
        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a ModelStateDictionary, " +
        "When CustomResponse is called, " +
        "Then it returns BadRequest")]
    [Trait("Type", nameof(MainController))]
    public async Task CustomResponse_ModelState_ReturnsBadRequest()
    {
        //Given
        var handler = new FakeNotificationHandler();
        var controller = CreateController(handler);
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("name", "required");

        //When
        var result = controller.CallCustomResponse(modelState);

        //Then
        result.Should().BeOfType<BadRequestObjectResult>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a ValidationResult, " +
        "When CustomResponse is called, " +
        "Then it returns BadRequest")]
    [Trait("Type", nameof(MainController))]
    public async Task CustomResponse_ValidationResult_ReturnsBadRequest()
    {
        //Given
        var handler = new FakeNotificationHandler();
        var controller = CreateController(handler);
        var validation = new ValidationResult(new[] { new ValidationFailure("name", "required") });

        //When
        var result = controller.CallCustomResponse(validation);

        //Then
        result.Should().BeOfType<BadRequestObjectResult>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a not found detail, " +
        "When NotFound is called, " +
        "Then it returns NotFoundObjectResult")]
    [Trait("Type", nameof(MainController))]
    public async Task NotFound_ReturnsNotFoundResponse()
    {
        //Given
        var handler = new FakeNotificationHandler();
        var controller = CreateController(handler);

        //When
        var result = controller.CallNotFound("missing");

        //Then
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeOfType<ApiResponse>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an exception, " +
        "When Notify is called, " +
        "Then it stores a notification")]
    [Trait("Type", nameof(MainController))]
    public async Task Notify_Exception_AddsNotification()
    {
        //Given
        var handler = new FakeNotificationHandler();
        var controller = CreateController(handler);

        //When
        controller.CallNotify(new InvalidOperationException("boom"));

        //Then
        handler.Get().Should().ContainSingle(x => x.Value == "boom");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given existing notifications, " +
        "When Clear is called, " +
        "Then it removes them")]
    [Trait("Type", nameof(MainController))]
    public async Task Clear_RemovesNotifications()
    {
        //Given
        var handler = new FakeNotificationHandler();
        var controller = CreateController(handler);
        controller.CallNotify("warn");

        //When
        controller.CallClear();

        //Then
        handler.Get().Should().BeEmpty();
        await Task.CompletedTask;
    }
}
