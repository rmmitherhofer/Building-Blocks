using System;
using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Xunit;
using Zypher.Secutiry.JwtSigningCredentials.Store.FileSystem;
using Zypher.Security.Jwt.Core;
using Zypher.Security.Jwt.Core.Jwa;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Security.JwtSigningCredentials.Store.FileSystem.Tests.Stores;

public class FileSystemStoreTests
{
    private static FileSystemStore CreateStore(out DirectoryInfo directory)
    {
        directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
        var options = Microsoft.Extensions.Options.Options.Create(new JwtOptions { CacheTime = TimeSpan.FromMinutes(1), KeyPrefix = "test-" });
        var cache = new MemoryCache(new MemoryCacheOptions());
        return new FileSystemStore(directory, options, cache);
    }

    private static KeyMaterial CreateKeyMaterial()
    {
        var crypto = new CryptographicKey(Algorithm.Create(AlgorithmType.RSA, JwtType.Jws));
        return new KeyMaterial(crypto);
    }

    [Fact(DisplayName =
        "Given a key material, " +
        "When Store is called, " +
        "Then it writes a current key file")]
    [Trait("Type", nameof(FileSystemStore))]
    public async Task Store_WritesFile()
    {
        //Given
        var store = CreateStore(out var directory);
        var key = CreateKeyMaterial();

        //When
        await store.Store(key);

        //Then
        directory.GetFiles("*.key").Should().NotBeEmpty();
        directory.Delete(true);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given stored keys, " +
        "When Get is called, " +
        "Then it returns the matching key")]
    [Trait("Type", nameof(FileSystemStore))]
    public async Task Get_ReturnsKey()
    {
        //Given
        var store = CreateStore(out var directory);
        var key = CreateKeyMaterial();
        await store.Store(key);

        //When
        var result = await store.Get(key.KeyId);

        //Then
        result.Should().NotBeNull();
        result!.KeyId.Should().Be(key.KeyId);
        directory.Delete(true);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given stored keys, " +
        "When GetLastKeys is called, " +
        "Then it returns limited list")]
    [Trait("Type", nameof(FileSystemStore))]
    public async Task GetLastKeys_ReturnsLimited()
    {
        //Given
        var store = CreateStore(out var directory);
        await store.Store(CreateKeyMaterial());
        await store.Store(CreateKeyMaterial());

        //When
        var keys = await store.GetLastKeys(1);

        //Then
        keys.Should().NotBeNull();
        keys!.Count.Should().Be(1);
        directory.Delete(true);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given stored keys, " +
        "When Clear is called, " +
        "Then it deletes key files")]
    [Trait("Type", nameof(FileSystemStore))]
    public async Task Clear_RemovesFiles()
    {
        //Given
        var store = CreateStore(out var directory);
        await store.Store(CreateKeyMaterial());

        //When
        await store.Clear();

        //Then
        directory.GetFiles("*.key").Should().BeEmpty();
        directory.Delete(true);
        await Task.CompletedTask;
    }
}
