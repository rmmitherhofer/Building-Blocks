using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Zypher.Responses;

namespace Zypher.Responses.Tests.Responses;

public class ApiResponseTests
{
    [Fact(DisplayName =
        "Given a not found response, " +
        "When ApiResponse is created, " +
        "Then it maps to not found issue")]
    [Trait("Type", nameof(ApiResponse))]
    public async Task ApiResponse_FromNotFound_MapsIssue()
    {
        //Given
        var notFound = new NotFoundResponse("missing");

        //When
        var response = new ApiResponse(notFound);

        //Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Issues.Should().ContainSingle();
        response.Issues.First().Type.Should().Be(IssuerResponseType.NotFound);
        response.Issues.First().Title.Should().Be("Not found");
        response.Issues.First().Details.Should().ContainSingle().Which.Value.Should().Be("missing");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a validation response, " +
        "When ApiResponse is created, " +
        "Then it maps to validation issue")]
    [Trait("Type", nameof(ApiResponse))]
    public async Task ApiResponse_FromValidation_MapsIssue()
    {
        //Given
        var validations = new List<NotificationResponse>
        {
            new() { Key = "name", Value = "required" }
        };
        var validation = new ValidationResponse(validations);

        //When
        var response = new ApiResponse(validation);

        //Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Issues.Should().ContainSingle();
        response.Issues.First().Type.Should().Be(IssuerResponseType.Validation);
        response.Issues.First().Details.Should().HaveCount(1);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a validation response and title, " +
        "When ApiResponse is created, " +
        "Then it sets the title")]
    [Trait("Type", nameof(ApiResponse))]
    public async Task ApiResponse_FromValidation_WithTitle()
    {
        //Given
        var validations = new List<NotificationResponse>
        {
            new() { Key = "name", Value = "required" }
        };
        var validation = new ValidationResponse(validations);

        //When
        var response = new ApiResponse(validation, "Invalid input");

        //Then
        response.Issues.Should().ContainSingle();
        response.Issues.First().Title.Should().Be("Invalid input");
        response.Issues.First().Type.Should().Be(IssuerResponseType.Validation);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a status code and validation response, " +
        "When ApiResponse is created, " +
        "Then it maps to error issue")]
    [Trait("Type", nameof(ApiResponse))]
    public async Task ApiResponse_FromValidationWithStatus_MapsError()
    {
        //Given
        var validations = new List<NotificationResponse>
        {
            new() { Key = "name", Value = "required" }
        };
        var validation = new ValidationResponse(validations);

        //When
        var response = new ApiResponse(HttpStatusCode.Conflict, validation);

        //Then
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        response.Issues.Should().ContainSingle();
        response.Issues.First().Type.Should().Be(IssuerResponseType.Error);
        response.Issues.First().Title.Should().Be("An error occurred");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an error response, " +
        "When ApiResponse is created, " +
        "Then it maps to error issue")]
    [Trait("Type", nameof(ApiResponse))]
    public async Task ApiResponse_FromError_MapsIssue()
    {
        //Given
        var error = new ErrorResponse(HttpStatusCode.InternalServerError, "boom");

        //When
        var response = new ApiResponse(error);

        //Then
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Issues.Should().ContainSingle();
        response.Issues.First().Type.Should().Be(IssuerResponseType.Error);
        response.Issues.First().Details.Should().ContainSingle().Which.Value.Should().Be("boom");
        await Task.CompletedTask;
    }
}
