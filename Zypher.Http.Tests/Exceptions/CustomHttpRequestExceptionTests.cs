using System;
using System.Net;
using FluentAssertions;
using Xunit;
using Zypher.Http.Exceptions;

namespace Zypher.Http.Tests.Exceptions;

public class CustomHttpRequestExceptionTests
{
    [Fact(DisplayName =
        "Given a status code, " +
        "When CustomHttpRequestException is created, " +
        "Then it stores the status code")]
    [Trait("Type", nameof(CustomHttpRequestException))]
    public async Task CustomHttpRequestException_WithStatusCode_StoresStatus()
    {
        //Given
        var status = HttpStatusCode.BadRequest;

        //When
        var exception = new CustomHttpRequestException(status);

        //Then
        exception.HttpStatusCode.Should().Be(status);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a status code and message, " +
        "When CustomHttpRequestException is created, " +
        "Then it stores the status and message")]
    [Trait("Type", nameof(CustomHttpRequestException))]
    public async Task CustomHttpRequestException_WithStatusAndMessage_StoresData()
    {
        //Given
        var status = HttpStatusCode.Unauthorized;
        var message = "unauthorized";

        //When
        var exception = new CustomHttpRequestException(status, message);

        //Then
        exception.HttpStatusCode.Should().Be(status);
        exception.Message.Should().Be(message);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a message and inner exception, " +
        "When CustomHttpRequestException is created, " +
        "Then it stores the inner exception")]
    [Trait("Type", nameof(CustomHttpRequestException))]
    public async Task CustomHttpRequestException_WithInnerException_StoresInner()
    {
        //Given
        var inner = new InvalidOperationException("inner");

        //When
        var exception = new CustomHttpRequestException("message", inner);

        //Then
        exception.InnerException.Should().BeSameAs(inner);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a default constructor, " +
        "When CustomHttpRequestException is created, " +
        "Then it initializes without message")]
    [Trait("Type", nameof(CustomHttpRequestException))]
    public async Task CustomHttpRequestException_DefaultConstructor_Works()
    {
        //Given
        var exception = new CustomHttpRequestException();

        //When
        var message = exception.Message;

        //Then
        message.Should().NotBeNullOrEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a message, " +
        "When CustomHttpRequestException is created, " +
        "Then it stores the message")]
    [Trait("Type", nameof(CustomHttpRequestException))]
    public async Task CustomHttpRequestException_WithMessage_StoresMessage()
    {
        //Given
        var message = "custom";

        //When
        var exception = new CustomHttpRequestException(message);

        //Then
        exception.Message.Should().Be(message);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given status, message, and inner exception, " +
        "When CustomHttpRequestException is created, " +
        "Then it stores all values")]
    [Trait("Type", nameof(CustomHttpRequestException))]
    public async Task CustomHttpRequestException_WithStatusMessageAndInner_StoresAll()
    {
        //Given
        var status = HttpStatusCode.InternalServerError;
        var message = "failure";
        var inner = new InvalidOperationException("inner");

        //When
        var exception = new CustomHttpRequestException(status, message, inner);

        //Then
        exception.HttpStatusCode.Should().Be(status);
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeSameAs(inner);
        await Task.CompletedTask;
    }
}
