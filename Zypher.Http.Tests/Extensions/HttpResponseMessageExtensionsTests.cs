using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Xunit;
using Zypher.Http.Exceptions;
using Zypher.Http.Extensions;

namespace Zypher.Http.Tests.Extensions;

public class HttpResponseMessageExtensionsTests
{
    private sealed class SampleResponse
    {
        public string? Name { get; set; }
    }

    [Fact(DisplayName =
        "Given an empty response content, " +
        "When ReadAsAsync is called, " +
        "Then it returns default")]
    [Trait("Type", nameof(HttpResponseMessageExtensions))]
    public async Task ReadAsAsync_EmptyContent_ReturnsDefault()
    {
        //Given
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
        };

        //When
        var result = await response.ReadAsAsync<SampleResponse>();

        //Then
        result.Should().BeNull("content was empty");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a whitespace response content, " +
        "When ReadAsAsync is called, " +
        "Then it returns default")]
    [Trait("Type", nameof(HttpResponseMessageExtensions))]
    public async Task ReadAsAsync_WhitespaceContent_ReturnsDefault()
    {
        //Given
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("  ", Encoding.UTF8, "application/json")
        };

        //When
        var result = await response.ReadAsAsync<SampleResponse>();

        //Then
        result.Should().BeNull("content was whitespace");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a JSON response content, " +
        "When ReadAsAsync is called, " +
        "Then it deserializes the object")]
    [Trait("Type", nameof(HttpResponseMessageExtensions))]
    public async Task ReadAsAsync_JsonContent_Deserializes()
    {
        //Given
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"Name\":\"John\"}", Encoding.UTF8, "application/json")
        };

        //When
        var result = await response.ReadAsAsync<SampleResponse>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //Then
        result.Should().NotBeNull("content was not empty");
        result!.Name.Should().Be("John", "content was '{0}'", await response.Content.ReadAsStringAsync());
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a bad request response, " +
        "When HasErrors is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(HttpResponseMessageExtensions))]
    public async Task HasErrors_BadRequest_ReturnsTrue()
    {
        //Given
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

        //When
        var result = response.HasErrors();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a forbidden response, " +
        "When HasErrors is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(HttpResponseMessageExtensions))]
    public async Task HasErrors_Forbidden_ReturnsTrue()
    {
        //Given
        var response = new HttpResponseMessage(HttpStatusCode.Forbidden);

        //When
        var result = response.HasErrors();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a bad gateway response, " +
        "When HasErrors is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(HttpResponseMessageExtensions))]
    public async Task HasErrors_BadGateway_ReturnsTrue()
    {
        //Given
        var response = new HttpResponseMessage(HttpStatusCode.BadGateway);

        //When
        var result = response.HasErrors();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an unauthorized response and throwException false, " +
        "When HasErrors is called, " +
        "Then it returns true without throwing")]
    [Trait("Type", nameof(HttpResponseMessageExtensions))]
    public async Task HasErrors_Unauthorized_NoThrow_ReturnsTrue()
    {
        //Given
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

        //When
        var result = response.HasErrors(false);

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an unauthorized response, " +
        "When HasErrors is called, " +
        "Then it throws CustomHttpRequestException")]
    [Trait("Type", nameof(HttpResponseMessageExtensions))]
    public async Task HasErrors_Unauthorized_Throws()
    {
        //Given
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            RequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://example.com/users")
        };

        //When
        Action action = () => response.HasErrors();

        //Then
        action.Should().Throw<CustomHttpRequestException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an internal server error response, " +
        "When HasErrors is called, " +
        "Then it throws CustomHttpRequestException")]
    [Trait("Type", nameof(HttpResponseMessageExtensions))]
    public async Task HasErrors_InternalServerError_Throws()
    {
        //Given
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            RequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://example.com/users")
        };

        //When
        Action action = () => response.HasErrors();

        //Then
        action.Should().Throw<CustomHttpRequestException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a not found response, " +
        "When HasErrors is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(HttpResponseMessageExtensions))]
    public async Task HasErrors_NotFound_ReturnsTrue()
    {
        //Given
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);

        //When
        var result = response.HasErrors();

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a success response, " +
        "When HasErrors is called, " +
        "Then it returns false")]
    [Trait("Type", nameof(HttpResponseMessageExtensions))]
    public async Task HasErrors_Ok_ReturnsFalse()
    {
        //Given
        var response = new HttpResponseMessage(HttpStatusCode.OK);

        //When
        var result = response.HasErrors();

        //Then
        result.Should().BeFalse();
        await Task.CompletedTask;
    }
}
