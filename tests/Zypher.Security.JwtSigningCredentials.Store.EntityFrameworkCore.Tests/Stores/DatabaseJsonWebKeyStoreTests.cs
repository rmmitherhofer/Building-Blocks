using System;
using System.Collections.ObjectModel;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Zypher.JwtSigningCredentials.Store.EntityFrameworkCore;
using Zypher.Security.Jwt.Core;
using Zypher.Security.Jwt.Core.Jwa;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Security.JwtSigningCredentials.Store.EntityFrameworkCore.Tests.Stores;

public class DatabaseJsonWebKeyStoreTests
{
    private sealed class TestDbContext : DbContext, ISecurityKeyContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        public DbSet<KeyMaterial> SecurityKeys { get; set; } = null!;
    }

    private static DatabaseJsonWebKeyStore<TestDbContext> CreateStore(out TestDbContext context)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        context = new TestDbContext(options);
        var jwtOptions = Microsoft.Extensions.Options.Options.Create(new JwtOptions { CacheTime = TimeSpan.FromMinutes(1), AlgorithmToKeep = 2 });
        var cache = new MemoryCache(new MemoryCacheOptions());
        var logger = new LoggerFactory().CreateLogger<DatabaseJsonWebKeyStore<TestDbContext>>();

        return new DatabaseJsonWebKeyStore<TestDbContext>(context, jwtOptions, cache, logger);
    }

    private static KeyMaterial CreateKeyMaterial()
    {
        var crypto = new CryptographicKey(Algorithm.Create(AlgorithmType.RSA, JwtType.Jws));
        return new KeyMaterial(crypto);
    }

    [Fact(DisplayName =
        "Given a key material, " +
        "When Store is called, " +
        "Then it persists the key")]
    [Trait("Type", nameof(DatabaseJsonWebKeyStore<TestDbContext>))]
    public async Task Store_PersistsKey()
    {
        //Given
        var store = CreateStore(out var context);
        var key = CreateKeyMaterial();

        //When
        await store.Store(key);

        //Then
        (await context.SecurityKeys.CountAsync()).Should().Be(1);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given stored keys, " +
        "When GetCurrent is called, " +
        "Then it returns latest non-revoked key")]
    [Trait("Type", nameof(DatabaseJsonWebKeyStore<TestDbContext>))]
    public async Task GetCurrent_ReturnsLatest()
    {
        //Given
        var store = CreateStore(out _);
        var older = CreateKeyMaterial();
        older.CreationDate = DateTime.UtcNow.AddDays(-1);
        var newer = CreateKeyMaterial();

        await store.Store(older);
        await store.Store(newer);

        //When
        var current = await store.GetCurrent();

        //Then
        current.Should().NotBeNull();
        current!.KeyId.Should().Be(newer.KeyId);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given stored keys, " +
        "When GetLastKeys is called, " +
        "Then it returns a limited list")]
    [Trait("Type", nameof(DatabaseJsonWebKeyStore<TestDbContext>))]
    public async Task GetLastKeys_ReturnsLimited()
    {
        //Given
        var store = CreateStore(out _);
        await store.Store(CreateKeyMaterial());
        await store.Store(CreateKeyMaterial());

        //When
        ReadOnlyCollection<KeyMaterial>? keys = await store.GetLastKeys(1);

        //Then
        keys.Should().NotBeNull();
        keys!.Count.Should().Be(1);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a key, " +
        "When Revoke is called, " +
        "Then it marks the key revoked")]
    [Trait("Type", nameof(DatabaseJsonWebKeyStore<TestDbContext>))]
    public async Task Revoke_MarksRevoked()
    {
        //Given
        var store = CreateStore(out _);
        var key = CreateKeyMaterial();
        await store.Store(key);

        //When
        await store.Revoke(key, "reason");

        //Then
        var current = await store.Get(key.KeyId);
        current.Should().NotBeNull();
        current!.IsRevoked.Should().BeTrue();
        current.RevokedReason.Should().Be("reason");
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given stored keys, " +
        "When Clear is called, " +
        "Then it removes all keys")]
    [Trait("Type", nameof(DatabaseJsonWebKeyStore<TestDbContext>))]
    public async Task Clear_RemovesAll()
    {
        //Given
        var store = CreateStore(out var context);
        await store.Store(CreateKeyMaterial());

        //When
        await store.Clear();

        //Then
        (await context.SecurityKeys.CountAsync()).Should().Be(0);
        await Task.CompletedTask;
    }
}
