using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Zypher.Api.Foundation.Middleware;
using Zypher.Domain.Exceptions;
using Zypher.Http.Exceptions;

namespace Zypher.Api.Foundation.Tests.Middleware;

public class ExceptionMiddlewareTests
{
    private sealed class TestLogger : ILogger<ExceptionMiddleware>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
        }
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/test";
        return context;
    }

    private static async Task<string> ReadBodyAsync(HttpResponse response)
    {
        response.Body.Position = 0;
        using var reader = new StreamReader(response.Body, Encoding.UTF8, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }

    [Fact(DisplayName =
        "Given a DomainException, " +
        "When middleware handles it, " +
        "Then it returns 400 and message")]
    [Trait("Type", nameof(ExceptionMiddleware))]
    public async Task InvokeAsync_DomainException_ReturnsBadRequest()
    {
        //Given
        var context = CreateContext();
        RequestDelegate next = _ => throw new DomainException("invalid");
        var middleware = new ExceptionMiddleware(next, new TestLogger());

        //When
        await middleware.InvokeAsync(context);

        //Then
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        context.Response.Headers["X-Original-Status-Code"].ToString().Should().Be("BadRequest");
        (await ReadBodyAsync(context.Response)).Should().Contain("invalid");
    }

    [Fact(DisplayName =
        "Given a NotFoundException, " +
        "When middleware handles it, " +
        "Then it returns 404 and message")]
    [Trait("Type", nameof(ExceptionMiddleware))]
    public async Task InvokeAsync_NotFoundException_ReturnsNotFound()
    {
        //Given
        var context = CreateContext();
        RequestDelegate next = _ => throw new NotFoundException("missing");
        var middleware = new ExceptionMiddleware(next, new TestLogger());

        //When
        await middleware.InvokeAsync(context);

        //Then
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        context.Response.Headers["X-Original-Status-Code"].ToString().Should().Be("NotFound");
        (await ReadBodyAsync(context.Response)).Should().Contain("missing");
    }

    [Fact(DisplayName =
        "Given a CustomHttpRequestException, " +
        "When middleware handles it, " +
        "Then it returns 502 and message")]
    [Trait("Type", nameof(ExceptionMiddleware))]
    public async Task InvokeAsync_CustomHttpRequestException_ReturnsBadGateway()
    {
        //Given
        var context = CreateContext();
        RequestDelegate next = _ => throw new CustomHttpRequestException("bad gateway");
        var middleware = new ExceptionMiddleware(next, new TestLogger());

        //When
        await middleware.InvokeAsync(context);

        //Then
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadGateway);
        context.Response.Headers["X-Original-Status-Code"].ToString().Should().Be("BadGateway");
        (await ReadBodyAsync(context.Response)).Should().Contain("bad gateway");
    }

    [Fact(DisplayName =
        "Given an unauthorized exception, " +
        "When middleware handles it, " +
        "Then it returns 401 and message")]
    [Trait("Type", nameof(ExceptionMiddleware))]
    public async Task InvokeAsync_Unauthorized_ReturnsUnauthorized()
    {
        //Given
        var context = CreateContext();
        RequestDelegate next = _ => throw new UnauthorizedAccessException("nope");
        var middleware = new ExceptionMiddleware(next, new TestLogger());

        //When
        await middleware.InvokeAsync(context);

        //Then
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        context.Response.Headers["X-Original-Status-Code"].ToString().Should().Be("Unauthorized");
        (await ReadBodyAsync(context.Response)).Should().Contain("nope");
    }

    [Fact(DisplayName =
        "Given a generic exception, " +
        "When middleware handles it, " +
        "Then it returns 500 and message")]
    [Trait("Type", nameof(ExceptionMiddleware))]
    public async Task InvokeAsync_Generic_ReturnsInternalServerError()
    {
        //Given
        var context = CreateContext();
        RequestDelegate next = _ => throw new Exception("boom");
        var middleware = new ExceptionMiddleware(next, new TestLogger());

        //When
        await middleware.InvokeAsync(context);

        //Then
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        context.Response.Headers["X-Original-Status-Code"].ToString().Should().Be("InternalServerError");
        (await ReadBodyAsync(context.Response)).Should().Contain("boom");
    }
}
