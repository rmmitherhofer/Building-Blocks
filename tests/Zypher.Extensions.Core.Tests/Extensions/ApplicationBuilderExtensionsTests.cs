using System;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zypher.Extensions.Core;

namespace Zypher.Extensions.Core.Tests.Extensions;

public class ApplicationBuilderExtensionsTests
{
    private sealed class DummyMiddleware
    {
        private readonly RequestDelegate _next;

        public DummyMiddleware(RequestDelegate next) => _next = next;

        public Task Invoke(HttpContext context) => _next(context);
    }

    [Fact(DisplayName =
        "Given an application builder, " +
        "When TryUseMiddleware is called twice, " +
        "Then it only registers the middleware once")]
    [Trait("Type", nameof(ApplicationBuilderExtensions))]
    public async Task TryUseMiddleware_AddsOnlyOnce()
    {
        //Given
        var provider = new ServiceCollection().BuildServiceProvider();
        var app = new ApplicationBuilder(provider);
        var key = typeof(DummyMiddleware).FullName!;

        //When
        app.TryUseMiddleware<DummyMiddleware>();
        app.TryUseMiddleware<DummyMiddleware>();

        //Then
        app.Properties.ContainsKey(key).Should().BeTrue();
        app.Properties[key].Should().Be(true);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null application builder, " +
        "When TryUseMiddleware is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(ApplicationBuilderExtensions))]
    public async Task TryUseMiddleware_NullApp_Throws()
    {
        //Given
        IApplicationBuilder? app = null;

        //When
        Action action = () => app!.TryUseMiddleware<DummyMiddleware>();

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }
}
