using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zypher.User.Configurations;
using Zypher.User.Users;

namespace Zypher.User.Tests.Configurations;

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
        "When AddAspNetUser is called, " +
        "Then it registers core services")]
    [Trait("Type", nameof(UserConfiguration))]
    public async Task AddAspNetUser_RegistersServices()
    {
        //Given
        var services = new ServiceCollection();

        //When
        services.AddAspNetUser();
        var provider = services.BuildServiceProvider();

        //Then
        provider.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Should().NotBeNull();
        provider.GetService<IAspNetUser>().Should().BeOfType<AspNetUser>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null service collection, " +
        "When AddAspNetUser is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(UserConfiguration))]
    public async Task AddAspNetUser_NullServices_Throws()
    {
        //Given
        IServiceCollection? services = null;

        //When
        Action action = () => services!.AddAspNetUser();

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an existing IAspNetUser registration, " +
        "When AddAspNetUser is called, " +
        "Then it does not replace the registration")]
    [Trait("Type", nameof(UserConfiguration))]
    public async Task AddAspNetUser_DoesNotReplaceExisting()
    {
        //Given
        var services = new ServiceCollection();
        services.AddScoped<IAspNetUser, FakeAspNetUser>();

        //When
        services.AddAspNetUser();
        var provider = services.BuildServiceProvider();

        //Then
        provider.GetService<IAspNetUser>().Should().BeOfType<FakeAspNetUser>();
        await Task.CompletedTask;
    }
}
