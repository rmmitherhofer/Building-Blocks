using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;
using Zypher.Extensions.Core;
using Zypher.Http.Extensions;

namespace Zypher.Http.Tests.Extensions;

public class HttpRequestMessageExtensionsTests
{
    [Fact(DisplayName =
        "Given a request and a payload, " +
        "When SetJsonContent is called, " +
        "Then it sets the JSON content")]
    [Trait("Type", nameof(HttpRequestMessageExtensions))]
    public async Task SetJsonContent_SetsJsonContent()
    {
        //Given
        var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com/users");

        //When
        request.SetJsonContent(new { Name = "John" }, new JsonSerializerOptions { WriteIndented = false });

        //Then
        request.Content.Should().NotBeNull("content should be set");
        var body = await request.Content!.ReadAsStringAsync();
        body.Should().Contain("\"Name\"", "content was '{0}'", body);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an existing header, " +
        "When AddHeader is called, " +
        "Then it does not overwrite the value")]
    [Trait("Type", nameof(HttpRequestMessageExtensions))]
    public async Task AddHeader_DoesNotOverwriteExistingValue()
    {
        //Given
        var request = new HttpRequestMessage();

        //When
        request.AddHeader("X-Test", "value-1");
        request.AddHeader("X-Test", "value-2");

        //Then
        request.Headers.GetValues("X-Test").Should().ContainSingle()
            .Which.Should().Be("value-1");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an existing header, " +
        "When AddOrUpdateHeader is called, " +
        "Then it replaces the value")]
    [Trait("Type", nameof(HttpRequestMessageExtensions))]
    public async Task AddOrUpdateHeader_ReplacesExistingValue()
    {
        //Given
        var request = new HttpRequestMessage();

        //When
        request.AddOrUpdateHeader("X-Test", "value-1");
        request.AddOrUpdateHeader("X-Test", "value-2");

        //Then
        request.Headers.GetValues("X-Test").Should().ContainSingle()
            .Which.Should().Be("value-2");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request with a header, " +
        "When GetHeader is called, " +
        "Then it returns the header value")]
    [Trait("Type", nameof(HttpRequestMessageExtensions))]
    public async Task GetHeader_ReturnsValue()
    {
        //Given
        var request = new HttpRequestMessage();
        request.Headers.Add("X-Test", "value-1");

        //When
        var result = request.GetHeader("X-Test");

        //Then
        result.Should().Be("value-1", "header was '{0}'", result);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request uri with query, " +
        "When GetQueryParameters is called, " +
        "Then it returns a dictionary of parameters")]
    [Trait("Type", nameof(HttpRequestMessageExtensions))]
    public async Task GetQueryParameters_ParsesQuery()
    {
        //Given
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/users?page=1&filter=all");

        //When
        var result = request.GetQueryParameters();

        //Then
        result.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            ["page"] = "1",
            ["filter"] = "all"
        });
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request without uri, " +
        "When GetQueryParameters is called, " +
        "Then it returns an empty dictionary")]
    [Trait("Type", nameof(HttpRequestMessageExtensions))]
    public async Task GetQueryParameters_NullUri_ReturnsEmpty()
    {
        //Given
        var request = new HttpRequestMessage();

        //When
        var result = request.GetQueryParameters();

        //Then
        result.Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request with headers and content, " +
        "When Clone is called, " +
        "Then it creates an equivalent copy")]
    [Trait("Type", nameof(HttpRequestMessageExtensions))]
    public async Task Clone_CopiesHeadersAndContent()
    {
        //Given
        var request = new HttpRequestMessage(HttpMethod.Put, "https://example.com/users/1");
        request.Headers.Add("X-Test", "value-1");
        request.Content = new StringContent("body", Encoding.UTF8, "text/plain");
        request.Content.Headers.ContentLanguage.Add("pt-BR");
        request.Version = new Version(2, 0);

        //When
        var clone = request.Clone();

        //Then
        clone.Method.Should().Be(HttpMethod.Put);
        clone.RequestUri.Should().Be(new Uri("https://example.com/users/1"));
        clone.Headers.GetValues("X-Test").Should().ContainSingle().Which.Should().Be("value-1");
        clone.Content.Should().NotBeNull();
        clone.Content!.Headers.ContentLanguage.Should().Contain("pt-BR");
        clone.Version.Should().Be(new Version(2, 0));

        var body = await clone.Content.ReadAsStringAsync();
        body.Should().Be("body", "content was '{0}'", body);
    }

    [Fact(DisplayName =
        "Given a request without content, " +
        "When Clone is called, " +
        "Then it copies headers and leaves content null")]
    [Trait("Type", nameof(HttpRequestMessageExtensions))]
    public async Task Clone_WithoutContent_LeavesContentNull()
    {
        //Given
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/users");
        request.Headers.Add("X-Test", "value-1");

        //When
        var clone = request.Clone();

        //Then
        clone.Headers.GetValues("X-Test").Should().ContainSingle().Which.Should().Be("value-1");
        clone.Content.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request and template, " +
        "When AddHeaderRequestTemplate is called, " +
        "Then it adds the template header")]
    [Trait("Type", nameof(HttpRequestMessageExtensions))]
    public async Task AddHeaderRequestTemplate_AddsHeader()
    {
        //Given
        var request = new HttpRequestMessage();
        HttpRequestMessageExtensions.Configure(new HttpContextAccessor { HttpContext = new DefaultHttpContext() });

        //When
        request.AddHeaderRequestTemplate("/users/{id}");

        //Then
        request.Headers.GetValues(HttpRequestMessageExtensions.X_REQUEST_TEMPLATE).Should().ContainSingle()
            .Which.Should().Be("/users/{id}");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty template, " +
        "When AddHeaderRequestTemplate is called, " +
        "Then it does not add the header")]
    [Trait("Type", nameof(HttpRequestMessageExtensions))]
    public async Task AddHeaderRequestTemplate_EmptyTemplate_DoesNotAddHeader()
    {
        //Given
        var request = new HttpRequestMessage();
        HttpRequestMessageExtensions.Configure(new HttpContextAccessor { HttpContext = new DefaultHttpContext() });

        //When
        request.AddHeaderRequestTemplate(string.Empty);

        //Then
        request.Headers.Contains(HttpRequestMessageExtensions.X_REQUEST_TEMPLATE).Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a context with headers, " +
        "When AddDefaultHeaders is called, " +
        "Then it adds the default headers to the request")]
    [Trait("Type", nameof(HttpRequestMessageExtensions))]
    public async Task AddDefaultHeaders_AddsHeadersFromContext()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Headers[HttpRequestExtensions.FORWARDED] = "10.0.0.1";
        context.Request.Headers[HttpRequestExtensions.USER_ID] = "user-123";
        context.Request.Headers[HttpRequestExtensions.CORRELATION_ID] = "corr-123";
        context.Request.Headers[HttpRequestExtensions.CLIENT_ID] = "client-abc";
        context.Request.Headers[HttpRequestExtensions.USER_AGENT] = "unit-test";
        context.Request.Headers[HttpRequestExtensions.USER_ACCOUNT] = "account-1";
        HttpRequestMessageExtensions.Configure(new HttpContextAccessor { HttpContext = context });

        var request = new HttpRequestMessage();

        //When
        request.AddDefaultHeaders();

        //Then
        request.Headers.GetValues(HttpRequestExtensions.FORWARDED).Should().ContainSingle()
            .Which.Should().Be("10.0.0.1");
        request.Headers.GetValues(HttpRequestExtensions.USER_ID).Should().ContainSingle()
            .Which.Should().Be("user-123");
        request.Headers.GetValues(HttpRequestExtensions.CORRELATION_ID).Should().ContainSingle()
            .Which.Should().Be("corr-123");
        request.Headers.GetValues(HttpRequestExtensions.USER_AGENT).Should().ContainSingle()
            .Which.Should().Be("unit-test");
        request.Headers.GetValues(HttpRequestExtensions.USER_ACCOUNT).Should().ContainSingle()
            .Which.Should().Be("account-1");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request uri and query params, " +
        "When AddQueryString is called, " +
        "Then it updates the request uri")]
    [Trait("Type", nameof(HttpRequestMessageExtensions))]
    public async Task AddQueryString_AppendsToRequestUri()
    {
        //Given
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/users");

        //When
        request.AddQueryString(new { Name = "John" });

        //Then
        request.RequestUri.Should().Be(new Uri("https://example.com/users?name=John"));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request without uri, " +
        "When AddQueryString is called, " +
        "Then it does nothing")]
    [Trait("Type", nameof(HttpRequestMessageExtensions))]
    public async Task AddQueryString_NullUri_DoesNothing()
    {
        //Given
        var request = new HttpRequestMessage();

        //When
        request.AddQueryString(new { Name = "John" });

        //Then
        request.RequestUri.Should().BeNull();
        await Task.CompletedTask;
    }
}
