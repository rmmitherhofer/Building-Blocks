using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;
using Zypher.Logs.Middlewares;

namespace Zypher.Logs.Tests.Middlewares;

[Collection("ConsoleTests")]
public class LogDecoratorMiddlewareTests
{
    private sealed class TestLogger : ILogger<LogDecoratorMiddleware>
    {
        public bool IsEnabled(LogLevel logLevel) => true;
        public IDisposable BeginScope<TState>(TState state) => new DummyScope();
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) { }

        private sealed class DummyScope : IDisposable
        {
            public void Dispose() { }
        }
    }

    [Fact(DisplayName =
        "Given the middleware, " +
        "When InvokeAsync is called, " +
        "Then it calls the next delegate")]
    [Trait("Type", nameof(LogDecoratorMiddleware))]
    public async Task InvokeAsync_CallsNext()
    {
        //Given
        var called = false;
        RequestDelegate next = _ =>
        {
            called = true;
            return Task.CompletedTask;
        };

        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/test";

        var middleware = new LogDecoratorMiddleware(new TestLogger(), next);

        //When
        await middleware.InvokeAsync(context);

        //Then
        called.Should().BeTrue();
    }

    [Fact(DisplayName =
        "Given the middleware, " +
        "When InvokeAsync is called, " +
        "Then it writes headers to console")]
    [Trait("Type", nameof(LogDecoratorMiddleware))]
    public async Task InvokeAsync_WritesToConsole()
    {
        //Given
        RequestDelegate next = _ => Task.CompletedTask;
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/test";

        var middleware = new LogDecoratorMiddleware(new TestLogger(), next);

        var original = Console.Out;
        var writer = new StringWriter(new StringBuilder());
        Console.SetOut(writer);

        try
        {
            //When
            await middleware.InvokeAsync(context);

            //Then
            writer.ToString().Should().Contain("Request-ID");
        }
        finally
        {
            Console.SetOut(original);
        }
    }

    [Fact(DisplayName =
        "Given an authenticated user, " +
        "When InvokeAsync is called, " +
        "Then it logs authenticated user fields")]
    [Trait("Type", nameof(LogDecoratorMiddleware))]
    public async Task InvokeAsync_AuthenticatedUser_LogsFields()
    {
        //Given
        RequestDelegate next = _ => Task.CompletedTask;
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/test";

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "id-1"),
            new Claim(ClaimTypes.Name, "name"),
            new Claim(ClaimTypes.Email, "email")
        }, "test", ClaimTypes.Name, ClaimTypes.Role);
        context.User = new ClaimsPrincipal(identity);

        var middleware = new LogDecoratorMiddleware(new TestLogger(), next);

        var original = Console.Out;
        var writer = new StringWriter(new StringBuilder());
        Console.SetOut(writer);

        try
        {
            //When
            await middleware.InvokeAsync(context);

            //Then
            var output = writer.ToString();
            output.Should().Contain("IsAuthenticated       : True");
            output.Should().Contain("User-ID");
            output.Should().Contain("User-Email");
        }
        finally
        {
            Console.SetOut(original);
        }
    }

    [Fact(DisplayName =
        "Given an unauthenticated user with request headers, " +
        "When InvokeAsync is called, " +
        "Then it logs request user fields")]
    [Trait("Type", nameof(LogDecoratorMiddleware))]
    public async Task InvokeAsync_Unauthenticated_LogsRequestFields()
    {
        //Given
        RequestDelegate next = _ => Task.CompletedTask;
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/test";
        context.Request.Headers[Zypher.Extensions.Core.HttpRequestExtensions.USER_ID] = "user-1";
        context.Request.Headers[Zypher.Extensions.Core.HttpRequestExtensions.USER_ACCOUNT] = "account-1";

        var middleware = new LogDecoratorMiddleware(new TestLogger(), next);

        var original = Console.Out;
        var writer = new StringWriter(new StringBuilder());
        Console.SetOut(writer);

        try
        {
            //When
            await middleware.InvokeAsync(context);

            //Then
            var output = writer.ToString();
            output.Should().Contain("IsAuthenticated       : False");
            output.Should().Contain("User-ID");
            output.Should().Contain("User-Account");
        }
        finally
        {
            Console.SetOut(original);
        }
    }
}
