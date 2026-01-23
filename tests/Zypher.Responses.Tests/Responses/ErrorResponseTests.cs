using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Responses;

namespace Zypher.Responses.Tests.Responses;

public class ErrorResponseTests
{
    [Fact(DisplayName =
        "Given a status code and message, " +
        "When ErrorResponse is created, " +
        "Then it sets status and message")]
    [Trait("Type", nameof(ErrorResponse))]
    public async Task ErrorResponse_Ctor_SetsValues()
    {
        //Given
        var statusCode = HttpStatusCode.BadRequest;

        //When
        var response = new ErrorResponse(statusCode, "failed");

        //Then
        response.StatusCode.Should().Be(statusCode);
        response.Message.Should().Be("failed");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty message, " +
        "When ErrorResponse is created, " +
        "Then it uses default message")]
    [Trait("Type", nameof(ErrorResponse))]
    public async Task ErrorResponse_EmptyMessage_UsesDefault()
    {
        //Given
        var statusCode = HttpStatusCode.InternalServerError;

        //When
        var response = new ErrorResponse(statusCode, string.Empty);

        //Then
        response.Message.Should().Be("An error occurred while processing your request.");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null message, " +
        "When ErrorResponse is created, " +
        "Then it uses default message")]
    [Trait("Type", nameof(ErrorResponse))]
    public async Task ErrorResponse_NullMessage_UsesDefault()
    {
        //Given
        var statusCode = HttpStatusCode.InternalServerError;

        //When
        var response = new ErrorResponse(statusCode, null!);

        //Then
        response.Message.Should().Be("An error occurred while processing your request.");
        await Task.CompletedTask;
    }
}
