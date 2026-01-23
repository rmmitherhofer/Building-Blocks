using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zypher.JwtSigningCredentials.Store.EntityFrameworkCore;
using Zypher.Security.Jwt.Core.Interfaces;

namespace Zypher.Security.JwtSigningCredentials.Store.EntityFrameworkCore.Tests.Extensions;

public class EFCoreServiceExtensionsTests
{
    private sealed class TestDbContext : DbContext, ISecurityKeyContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        public DbSet<Zypher.Security.Jwt.Core.Models.KeyMaterial> SecurityKeys { get; set; } = null!;
    }

    private sealed class FakeBuilder : IJwksBuilder
    {
        public FakeBuilder(IServiceCollection services) => Services = services;
        public IServiceCollection Services { get; }
    }

    [Fact(DisplayName =
        "Given a jwks builder, " +
        "When PersistKeysToDatabaseStore is called, " +
        "Then it registers the database store")]
    [Trait("Type", nameof(EFCoreServiceExtensions))]
    public async Task PersistKeysToDatabaseStore_RegistersStore()
    {
        //Given
        var services = new ServiceCollection();
        var builder = new FakeBuilder(services);

        //When
        builder.PersistKeysToDatabaseStore<TestDbContext>();

        //Then
        services.Should().Contain(x => x.ServiceType == typeof(IJsonWebKeyStore));
        await Task.CompletedTask;
    }
}
