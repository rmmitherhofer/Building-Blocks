using System;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;
using Zypher.Security.Jwt.Core.Interfaces;
using Zypher.Security.JwtSigningCredentials.AspNetCore;

namespace Zypher.Security.JwtSigningCredentials.AspNetCore.Tests.Extensions;

public class AspNetBuilderExtensionsTests
{
    private sealed class FakeBuilder : IJwksBuilder
    {
        public FakeBuilder(IServiceCollection services) => Services = services;
        public IServiceCollection Services { get; }
    }

    [Fact(DisplayName =
        "Given a jwks endpoint without slash, " +
        "When UseJwksDiscovery is called, " +
        "Then it throws ArgumentException")]
    [Trait("Type", nameof(AspNetBuilderExtensions))]
    public async Task UseJwksDiscovery_InvalidPath_Throws()
    {
        //Given
        var app = new ApplicationBuilder(new ServiceCollection().BuildServiceProvider());

        //When
        Action action = () => app.UseJwksDiscovery("jwks");

        //Then
        action.Should().Throw<ArgumentException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a jwks builder, " +
        "When UseJwtValidation is called, " +
        "Then it registers post configure options")]
    [Trait("Type", nameof(AspNetBuilderExtensions))]
    public async Task UseJwtValidation_RegistersPostConfigure()
    {
        //Given
        var services = new ServiceCollection();
        var builder = new FakeBuilder(services);

        //When
        builder.UseJwtValidation();

        //Then
        services.Should().Contain(x => x.ServiceType == typeof(IPostConfigureOptions<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>));
        await Task.CompletedTask;
    }
}
