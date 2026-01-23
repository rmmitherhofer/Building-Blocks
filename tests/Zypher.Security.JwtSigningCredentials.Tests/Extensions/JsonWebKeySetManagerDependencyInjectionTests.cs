using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zypher.Security.JwtSigningCredentials;
using Zypher.Security.JwtSigningCredentials.Interfaces;

namespace Zypher.Security.JwtSigningCredentials.Tests.Extensions;

public class JsonWebKeySetManagerDependencyInjectionTests
{
    [Fact(DisplayName =
        "Given a service collection, " +
        "When AddJwksManager is called, " +
        "Then it registers services and returns a builder")]
    [Trait("Type", nameof(JsonWebKeySetManagerDependencyInjection))]
    public async Task AddJwksManager_RegistersServices()
    {
        //Given
        var services = new ServiceCollection();

        //When
        var builder = services.AddJwksManager();

        //Then
        builder.Should().NotBeNull();
        services.Should().Contain(x => x.ServiceType == typeof(IJsonWebKeySetService));
        services.Should().Contain(x => x.ServiceType == typeof(IJsonWebKeyStore));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a builder, " +
        "When PersistKeysMemory is called, " +
        "Then it adds another store registration")]
    [Trait("Type", nameof(JsonWebKeySetManagerDependencyInjection))]
    public async Task PersistKeysMemory_AddsRegistration()
    {
        //Given
        var services = new ServiceCollection();
        var builder = services.AddJwksManager();
        var before = services.Count(x => x.ServiceType == typeof(IJsonWebKeyStore));

        //When
        builder.PersistKeysMemory();

        //Then
        services.Count(x => x.ServiceType == typeof(IJsonWebKeyStore)).Should().BeGreaterThan(before);
        await Task.CompletedTask;
    }
}
