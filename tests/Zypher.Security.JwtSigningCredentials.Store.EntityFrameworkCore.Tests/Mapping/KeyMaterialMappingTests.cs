using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xunit;
using Zypher.JwtSigningCredentials.Store.EntityFrameworkCore;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Security.JwtSigningCredentials.Store.EntityFrameworkCore.Tests.Mapping;

public class KeyMaterialMappingTests
{
    private sealed class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        public DbSet<KeyMaterial> SecurityKeys => Set<KeyMaterial>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new KeyMaterialMapping().Configure(modelBuilder.Entity<KeyMaterial>());
        }
    }

    [Fact(DisplayName =
        "Given KeyMaterialMapping, " +
        "When model is built, " +
        "Then it configures properties")]
    [Trait("Type", nameof(KeyMaterialMapping))]
    public async Task KeyMaterialMapping_ConfiguresProperties()
    {
        //Given
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(nameof(KeyMaterialMapping_ConfiguresProperties))
            .Options;

        //When
        using var context = new TestDbContext(options);
        var entity = context.Model.FindEntityType(typeof(KeyMaterial));

        //Then
        entity.Should().NotBeNull();
        var parameters = entity!.FindProperty(nameof(KeyMaterial.Parameters));
        parameters.Should().NotBeNull();
        parameters!.GetMaxLength().Should().Be(8000);
        parameters.IsNullable.Should().BeFalse();
        await Task.CompletedTask;
    }
}
