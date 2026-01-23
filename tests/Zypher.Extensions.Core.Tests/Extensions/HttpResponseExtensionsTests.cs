using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;
using Zypher.Extensions.Core;

namespace Zypher.Extensions.Core.Tests.Extensions;

public class HttpResponseExtensionsTests
{
    [Fact(DisplayName =
        "Given a response and request correlation id, " +
        "When SetCorrelationId is called, " +
        "Then it sets the response header")]
    [Trait("Type", nameof(HttpResponseExtensions))]
    public async Task SetCorrelationId_UsesCorrelationId()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Headers[HttpRequestExtensions.CORRELATION_ID] = "corr-123";

        //When
        context.Response.SetCorrelationId();

        //Then
        context.Response.Headers[HttpResponseExtensions.CORRELATION_ID].ToString().Should().Be("corr-123");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a response and request id only, " +
        "When SetCorrelationId is called, " +
        "Then it uses the request id")]
    [Trait("Type", nameof(HttpResponseExtensions))]
    public async Task SetCorrelationId_UsesRequestId()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Request.Headers[HttpRequestExtensions.REQUEST_ID] = "req-123";

        //When
        context.Response.SetCorrelationId();

        //Then
        context.Response.Headers[HttpResponseExtensions.CORRELATION_ID].ToString().Should().Be("req-123");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a response and header, " +
        "When AddHeader is called, " +
        "Then it adds the header once")]
    [Trait("Type", nameof(HttpResponseExtensions))]
    public async Task AddHeader_AddsOnce()
    {
        //Given
        var context = new DefaultHttpContext();

        //When
        context.Response.AddHeader("X-Test", "one");
        context.Response.AddHeader("X-Test", "two");

        //Then
        context.Response.Headers["X-Test"].ToString().Should().Be("one");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty header key, " +
        "When AddHeader is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(HttpResponseExtensions))]
    public async Task AddHeader_EmptyKey_Throws()
    {
        //Given
        var context = new DefaultHttpContext();

        //When
        Action action = () => context.Response.AddHeader(string.Empty, "value");

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a response and header, " +
        "When AddOrUpdateHeader is called, " +
        "Then it updates the header")]
    [Trait("Type", nameof(HttpResponseExtensions))]
    public async Task AddOrUpdateHeader_Updates()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Response.Headers["X-Test"] = "one";

        //When
        context.Response.AddOrUpdateHeader("X-Test", "two");

        //Then
        context.Response.Headers["X-Test"].ToString().Should().Be("two");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty header key, " +
        "When AddOrUpdateHeader is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(HttpResponseExtensions))]
    public async Task AddOrUpdateHeader_EmptyKey_Throws()
    {
        //Given
        var context = new DefaultHttpContext();

        //When
        Action action = () => context.Response.AddOrUpdateHeader(string.Empty, "value");

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a response with a header, " +
        "When GetHeader is called, " +
        "Then it returns the header value")]
    [Trait("Type", nameof(HttpResponseExtensions))]
    public async Task GetHeader_ReturnsValue()
    {
        //Given
        var context = new DefaultHttpContext();
        context.Response.Headers["X-Test"] = "value";

        //When
        var result = context.Response.GetHeader("X-Test");

        //Then
        result.Should().Be("value");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an empty header key, " +
        "When GetHeader is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(HttpResponseExtensions))]
    public async Task GetHeader_EmptyKey_Throws()
    {
        //Given
        var context = new DefaultHttpContext();

        //When
        Action action = () => context.Response.GetHeader(string.Empty);

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }
}
