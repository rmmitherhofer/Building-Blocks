using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;
using Zypher.Extensions.Core;
using Zypher.Http.Extensions;

namespace Zypher.Http.Tests.Extensions;

public class HttpClientExtensionsTests
{
    [Fact(DisplayName =
        "Given a bearer token, " +
        "When AddBearerToken is called, " +
        "Then it sets the Authorization header")]
    [Trait("Type", nameof(HttpClientExtensions))]
    public async Task AddBearerToken_SetsAuthorizationHeader()
    {
        //Given
        var client = new HttpClient();

        //When
        client.AddBearerToken("token-123");

        //Then
        client.DefaultRequestHeaders.Authorization.Should().Be(
            new AuthenticationHeaderValue("Bearer", "token-123"),
            "authorization was '{0}'",
            client.DefaultRequestHeaders.Authorization);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an existing header, " +
        "When AddHeader is called, " +
        "Then it does not overwrite the value")]
    [Trait("Type", nameof(HttpClientExtensions))]
    public async Task AddHeader_DoesNotOverwriteExistingValue()
    {
        //Given
        var client = new HttpClient();

        //When
        client.AddHeader("X-Test", "value-1");
        client.AddHeader("X-Test", "value-2");

        //Then
        client.DefaultRequestHeaders.GetValues("X-Test").Should().ContainSingle()
            .Which.Should().Be("value-1");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an existing header, " +
        "When AddOrUpdateHeader is called, " +
        "Then it replaces the value")]
    [Trait("Type", nameof(HttpClientExtensions))]
    public async Task AddOrUpdateHeader_ReplacesExistingValue()
    {
        //Given
        var client = new HttpClient();

        //When
        client.AddOrUpdateHeader("X-Test", "value-1");
        client.AddOrUpdateHeader("X-Test", "value-2");

        //Then
        client.DefaultRequestHeaders.GetValues("X-Test").Should().ContainSingle()
            .Which.Should().Be("value-2");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a template and base address, " +
        "When AddHeaderRequestTemplate is called, " +
        "Then it adds the header with the full template url")]
    [Trait("Type", nameof(HttpClientExtensions))]
    public async Task AddHeaderRequestTemplate_AddsHeaderWithBaseAddress()
    {
        //Given
        var client = new HttpClient { BaseAddress = new Uri("https://example.com") };
        HttpClientExtensions.Configure(new HttpContextAccessor { HttpContext = new DefaultHttpContext() });

        //When
        client.AddHeaderRequestTemplate("/users/{id}");

        //Then
        client.DefaultRequestHeaders.GetValues(HttpClientExtensions.X_REQUEST_TEMPLATE).Should().ContainSingle()
            .Which.Should().Be("https://example.com//users/{id}");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty template, " +
        "When AddHeaderRequestTemplate is called, " +
        "Then it does not add the header")]
    [Trait("Type", nameof(HttpClientExtensions))]
    public async Task AddHeaderRequestTemplate_EmptyTemplate_DoesNotAddHeader()
    {
        //Given
        var client = new HttpClient { BaseAddress = new Uri("https://example.com") };
        HttpClientExtensions.Configure(new HttpContextAccessor { HttpContext = new DefaultHttpContext() });

        //When
        client.AddHeaderRequestTemplate(string.Empty);

        //Then
        client.DefaultRequestHeaders.Contains(HttpClientExtensions.X_REQUEST_TEMPLATE).Should().BeFalse();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given no configured accessor, " +
        "When AddDefaultHeaders is called, " +
        "Then it does not add any headers")]
    [Trait("Type", nameof(HttpClientExtensions))]
    public async Task AddDefaultHeaders_NoAccessor_DoesNotAddHeaders()
    {
        //Given
        ResetAccessor();
        var client = new HttpClient();

        //When
        client.AddDefaultHeaders();

        //Then
        client.DefaultRequestHeaders.Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a request id but no correlation id, " +
        "When AddHeaderCorrelationId is called, " +
        "Then it uses the request id")]
    [Trait("Type", nameof(HttpClientExtensions))]
    public async Task AddHeaderCorrelationId_UsesRequestIdWhenMissingCorrelationId()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Headers[HttpRequestExtensions.REQUEST_ID] = "req-123";
        HttpClientExtensions.Configure(new HttpContextAccessor { HttpContext = context });

        var client = new HttpClient();

        //When
        client.AddHeaderCorrelationId();

        //Then
        client.DefaultRequestHeaders.GetValues(HttpRequestExtensions.CORRELATION_ID).Should().ContainSingle()
            .Which.Should().Be("req-123");
        await Task.CompletedTask;
    }

    private static void ResetAccessor()
    {
        var field = typeof(HttpClientExtensions).GetField("_accessor", BindingFlags.Static | BindingFlags.NonPublic);
        field?.SetValue(null, null);
    }

    [Fact(DisplayName =
        "Given a context with headers, " +
        "When AddDefaultHeaders is called, " +
        "Then it adds the default headers to the client")]
    [Trait("Type", nameof(HttpClientExtensions))]
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
        HttpClientExtensions.Configure(new HttpContextAccessor { HttpContext = context });

        var client = new HttpClient();

        //When
        client.AddDefaultHeaders();

        //Then
        client.DefaultRequestHeaders.GetValues(HttpRequestExtensions.FORWARDED).Should().ContainSingle()
            .Which.Should().Be("10.0.0.1");
        client.DefaultRequestHeaders.GetValues(HttpRequestExtensions.USER_ID).Should().ContainSingle()
            .Which.Should().Be("user-123");
        client.DefaultRequestHeaders.GetValues(HttpRequestExtensions.CORRELATION_ID).Should().ContainSingle()
            .Which.Should().Be("corr-123");

        var entryAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
        client.DefaultRequestHeaders.GetValues(HttpRequestExtensions.CLIENT_ID).Should().ContainSingle()
            .Which.Should().Be($"client-abc;{entryAssemblyName}");

        client.DefaultRequestHeaders.GetValues(HttpRequestExtensions.USER_AGENT).Should().ContainSingle()
            .Which.Should().Be("unit-test");
        client.DefaultRequestHeaders.GetValues(HttpRequestExtensions.USER_ACCOUNT).Should().ContainSingle()
            .Which.Should().Be("account-1");

        client.DefaultRequestHeaders.Should().ContainKey(HttpRequestExtensions.POD_NAME);
        await Task.CompletedTask;
    }
}
