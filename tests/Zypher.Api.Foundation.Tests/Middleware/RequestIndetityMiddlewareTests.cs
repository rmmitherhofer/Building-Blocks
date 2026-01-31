using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;
using Zypher.Api.Foundation.Middleware;

namespace Zypher.Api.Foundation.Tests.Middleware;

public class RequestIndetityMiddlewareTests
{
    [Fact(DisplayName =
        "Given a request, " +
        "When RequestIndetityMiddleware is invoked, " +
        "Then it calls the next delegate")]
    [Trait("Type", nameof(RequestIndetityMiddleware))]
    public async Task InvokeAsync_CallsNext()
    {
        //Given
        var called = false;
        RequestDelegate next = _ =>
        {
            called = true;
            return Task.CompletedTask;
        };
        var middleware = new RequestIndetityMiddleware(next);
        var context = new DefaultHttpContext();

        //When
        await middleware.InvokeAsync(context);

        //Then
        called.Should().BeTrue();
    }
}
