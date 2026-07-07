using System;
using System.Net;
using FluentAssertions;
using Xunit;
using Zypher.Domain.Exceptions;

namespace Zypher.Domain.Exceptions.Tests.Exceptions;

public class DomainExceptionTests
{
    [Fact(DisplayName =
        "Given a default DomainException, " +
        "When it is created, " +
        "Then it has BadRequest status code")]
    [Trait("Type", nameof(DomainException))]
    public async Task DomainException_Default_StatusCode()
    {
        //Given
        var exception = new DomainException();

        //When
        var status = exception.StatusCode;

        //Then
        status.Should().Be(HttpStatusCode.BadRequest);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a message, " +
        "When DomainException is created, " +
        "Then it stores the message")]
    [Trait("Type", nameof(DomainException))]
    public async Task DomainException_WithMessage_StoresMessage()
    {
        //Given
        var message = "error";

        //When
        var exception = new DomainException(message);

        //Then
        exception.Message.Should().Be(message);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a message and inner exception, " +
        "When DomainException is created, " +
        "Then it stores the inner exception")]
    [Trait("Type", nameof(DomainException))]
    public async Task DomainException_WithInner_StoresInner()
    {
        //Given
        var inner = new InvalidOperationException("inner");

        //When
        var exception = new DomainException("message", inner);

        //Then
        exception.InnerException.Should().BeSameAs(inner);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a DomainException, " +
        "When StatusCode is set, " +
        "Then it updates the value")]
    [Trait("Type", nameof(DomainException))]
    public async Task DomainException_StatusCode_Settable()
    {
        //Given
        var exception = new DomainException();

        //When
        exception.StatusCode = HttpStatusCode.Conflict;

        //Then
        exception.StatusCode.Should().Be(HttpStatusCode.Conflict);
        await Task.CompletedTask;
    }
}
