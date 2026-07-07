using System.IO;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zypher.Secutiry.JwtSigningCredentials.Store.FileSystem;
using Zypher.Security.Jwt.Core.Interfaces;

namespace Zypher.Security.JwtSigningCredentials.Store.FileSystem.Tests.Extensions;

public class FileSystemStoreExtensionsTests
{
    private sealed class FakeBuilder : IJwksBuilder
    {
        public FakeBuilder(IServiceCollection services) => Services = services;
        public IServiceCollection Services { get; }
    }

    [Fact(DisplayName =
        "Given a jwks builder, " +
        "When PersistKeysToFileSystem is called, " +
        "Then it registers the store")]
    [Trait("Type", nameof(FileSystemStoreExtensions))]
    public async Task PersistKeysToFileSystem_RegistersStore()
    {
        //Given
        var services = new ServiceCollection();
        var builder = new FakeBuilder(services);
        var directory = new DirectoryInfo(Path.GetTempPath());

        //When
        builder.PersistKeysToFileSystem(directory);

        //Then
        services.Should().Contain(x => x.ServiceType == typeof(IJsonWebKeyStore));
        await Task.CompletedTask;
    }
}
