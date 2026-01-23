using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zypher.Domain.Core.Configurations;
using Zypher.Domain.Core.Users;

namespace Zypher.Domain.Core.Tests.Configurations;

public class CoreConfigurationTests
{
    private sealed class FakeAspNetUser : IAspNetUser
    {
        public string Name => "fake";
        public string Id => "1";
        public string Email => "fake@example.com";
        public string? AccountCode => null;
        public string? Account => null;
        public string? Document => null;
        public string? Departament => null;
        public bool IsAuthenticated => false;
        public bool IsInRole(string role) => false;
        public IEnumerable<System.Security.Claims.Claim> GetClaims() => [];
    }

    [Fact(DisplayName =
        "Given a service collection, " +
        "When AddDomainCoreServices is called, " +
        "Then it registers core services")]
    [Trait("Type", nameof(CoreConfiguration))]
    public async Task AddDomainCoreServices_RegistersServices()
    {
        //Given
        var services = new ServiceCollection();

        //When
        services.AddDomainCoreServices();
        var provider = services.BuildServiceProvider();

        //Then
        provider.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Should().NotBeNull();
        provider.GetService<IAspNetUser>().Should().BeOfType<AspNetUser>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null service collection, " +
        "When AddDomainCoreServices is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(CoreConfiguration))]
    public async Task AddDomainCoreServices_NullServices_Throws()
    {
        //Given
        IServiceCollection? services = null;

        //When
        Action action = () => services!.AddDomainCoreServices();

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an existing IAspNetUser registration, " +
        "When AddDomainCoreServices is called, " +
        "Then it does not replace the registration")]
    [Trait("Type", nameof(CoreConfiguration))]
    public async Task AddDomainCoreServices_DoesNotReplaceExisting()
    {
        //Given
        var services = new ServiceCollection();
        services.AddScoped<IAspNetUser, FakeAspNetUser>();

        //When
        services.AddDomainCoreServices();
        var provider = services.BuildServiceProvider();

        //Then
        provider.GetService<IAspNetUser>().Should().BeOfType<FakeAspNetUser>();
        await Task.CompletedTask;
    }
}
