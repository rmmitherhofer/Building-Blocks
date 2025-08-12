using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Zypher.Security.Jwt.Core;
using Zypher.Security.Jwt.Core.Interfaces;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Secutiry.JwtSigningCredentials.Store.FileSystem;

public class FileSystemStore : IJsonWebKeyStore
{
    private readonly IOptions<JwtOptions> _options;
    private readonly IMemoryCache _memoryCache;
    public DirectoryInfo KeysPath { get; }
    public FileSystemStore(DirectoryInfo keysPath, IOptions<JwtOptions> options, IMemoryCache memoryCache)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

        KeysPath = keysPath;

        if (!KeysPath.Exists) KeysPath.Create();

    }
    public Task Clear()
    {
        if (KeysPath.Exists)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            foreach (var fileInfo in KeysPath.GetFiles("*.key"))
                fileInfo.Delete();
        }
        return Task.CompletedTask;
    }

    public Task<KeyMaterial?> Get(string keyId)
    {
        var files = Directory.GetFiles(KeysPath.FullName, $"*{keyId}*.key");

        if (files.Length != 0)
            return Task.FromResult(GetKey(files.First()))!;

        return Task.FromResult(null as KeyMaterial);
    }

    public Task<KeyMaterial?> GetCurrent()
    {
        if (!_memoryCache.TryGetValue(JwkConstants.CurrentJwkCache, out KeyMaterial? keyMaterial))
        {
            var currentFile = GetCurrentFile();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_options.Value.CacheTime);

            if (keyMaterial is not null)
                _memoryCache.Set(JwkConstants.CurrentJwkCache, keyMaterial, cacheEntryOptions);
        }
        return Task.FromResult(keyMaterial);
    }

    public Task<ReadOnlyCollection<KeyMaterial>?> GetLastKeys(int quantity)
    {
        if (!_memoryCache.TryGetValue(JwkConstants.JwksCache, out IReadOnlyCollection<KeyMaterial>? keys))
        {
            var files = KeysPath.GetFiles("*.key")
                .Take(quantity)
                .Select(f => f.FullName)
                .Select(GetKey)
                .ToList()
                .AsReadOnly();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_options.Value.CacheTime);

            if (files.Count != 0)
                _memoryCache.Set(JwkConstants.JwksCache, keys, cacheEntryOptions);
        }
        return Task.FromResult(keys?.ToList().AsReadOnly());
    }

    public async Task Revoke(KeyMaterial keyMaterial, string? reason = null)
    {
        if (keyMaterial is null) return;

        keyMaterial?.Revoke();

        foreach (var fileInfo in KeysPath.GetFiles("*.key"))
        {
            var key = GetKey(fileInfo.FullName);

            if (key.Id != keyMaterial?.Id) continue;

            await File.WriteAllTextAsync(
                fileInfo.FullName,
                JsonSerializer.Serialize(key, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull })
            );
            break;
        }
        ClearCache();
    }

    public async Task Store(KeyMaterial keyMaterial)
    {
        if (!KeysPath.Exists) KeysPath.Create();

        if (File.Exists(GetCurrentFile()))
            File.Copy(GetCurrentFile(), Path.Combine(KeysPath.FullName, $"{_options.Value.KeyPrefix}old-{DateTime.Now:yyyy-MM-dd}-{keyMaterial.KeyId}.key"));

        await File.WriteAllTextAsync(
            Path.Combine(KeysPath.FullName, $"{_options.Value.KeyPrefix}current-{keyMaterial.KeyId}.key"),
            JsonSerializer.Serialize(keyMaterial, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull })
            );

        ClearCache();

    }

    public bool NeedsUpdate() => !File.Exists(GetCurrentFile()) ||
        File.GetCreationTimeUtc(GetCurrentFile()).AddDays(_options.Value.DaysUntilExpire) < DateTime.UtcNow.Date;

    private KeyMaterial GetKey(string file)
    {
        if (!File.Exists(file))
            throw new FileNotFoundException($"Check configuration - Key file not found: {file}");

        return JsonSerializer.Deserialize<KeyMaterial>(File.ReadAllText(file))!;
    }

    private string GetCurrentFile()
    {
        var files = Directory.GetFiles(KeysPath.FullName, $"*current*.key");

        if (files.Any()) return Path.Combine(KeysPath.FullName, files.First());

        return Path.Combine(KeysPath.FullName, $"{_options.Value.KeyPrefix}current.key");
    }

    private void ClearCache()
    {
        _memoryCache.Remove(JwkConstants.JwksCache);
        _memoryCache.Remove(JwkConstants.CurrentJwkCache);
    }
}
