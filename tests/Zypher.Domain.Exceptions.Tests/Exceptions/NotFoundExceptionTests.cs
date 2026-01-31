using System;
using System.Net;
using FluentAssertions;
using Xunit;
using Zypher.Domain.Exceptions;

namespace Zypher.Domain.Exceptions.Tests.Exceptions;

public class NotFoundExceptionTests
{
    [Fact(DisplayName =
        "Given a default NotFoundException, " +
        "When it is created, " +
        "Then it has NotFound status code")]
    [Trait("Type", nameof(NotFoundException))]
    public async Task NotFoundException_Default_StatusCode()
    {
        //Given
        var exception = new NotFoundException();

        //When
        var status = exception.StatusCode;

        //Then
        status.Should().Be(HttpStatusCode.NotFound);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a message, " +
        "When NotFoundException is created, " +
        "Then it stores the message")]
    [Trait("Type", nameof(NotFoundException))]
    public async Task NotFoundException_WithMessage_StoresMessage()
    {
        //Given
        var message = "not found";

        //When
        var exception = new NotFoundException(message);

        //Then
        exception.Message.Should().Be(message);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a message and inner exception, " +
        "When NotFoundException is created, " +
        "Then it stores the inner exception")]
    [Trait("Type", nameof(NotFoundException))]
    public async Task NotFoundException_WithInner_StoresInner()
    {
        //Given
        var inner = new InvalidOperationException("inner");

        //When
        var exception = new NotFoundException("message", inner);

        //Then
        exception.InnerException.Should().BeSameAs(inner);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a NotFoundException, " +
        "When StatusCode is set, " +
        "Then it updates the value")]
    [Trait("Type", nameof(NotFoundException))]
    public async Task NotFoundException_StatusCode_Settable()
    {
        //Given
        var exception = new NotFoundException();

        //When
        exception.StatusCode = HttpStatusCode.Gone;

        //Then
        exception.StatusCode.Should().Be(HttpStatusCode.Gone);
        await Task.CompletedTask;
    }
}
