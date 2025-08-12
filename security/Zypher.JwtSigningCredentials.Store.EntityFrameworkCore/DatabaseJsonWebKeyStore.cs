using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using Zypher.Security.Jwt.Core;
using Zypher.Security.Jwt.Core.Interfaces;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.JwtSigningCredentials.Store.EntityFrameworkCore;

public class DatabaseJsonWebKeyStore<TDbContext> : IJsonWebKeyStore where TDbContext : DbContext, ISecurityKeyContext
{
    private readonly TDbContext _dbContext;
    private readonly IOptions<JwtOptions> _options;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<DatabaseJsonWebKeyStore<TDbContext>> _logger;

    public DatabaseJsonWebKeyStore(TDbContext dbContext, IOptions<JwtOptions> options, IMemoryCache memoryCache, ILogger<DatabaseJsonWebKeyStore<TDbContext>> logger)
    {
        _dbContext = dbContext;
        _options = options;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task Clear()
    {
        foreach (var key in _dbContext.SecurityKeys)
            _dbContext.SecurityKeys.Remove(key);

        await _dbContext.SaveChangesAsync();

        ClearCache();
    }

    public async Task<KeyMaterial?> Get(string keyId) => await _dbContext.SecurityKeys.FirstOrDefaultAsync(x => x.KeyId == keyId);

    public async Task<KeyMaterial?> GetCurrent()
    {
        if (!_memoryCache.TryGetValue(JwkConstants.CurrentJwkCache, out KeyMaterial? credentials))
        {
            credentials = await _dbContext.SecurityKeys.Where(x => !x.IsRevoked)
                .OrderByDescending(x => x.CreationDate)
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefaultAsync();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_options.Value.CacheTime);

            if (credentials is not null)
                _memoryCache.Set(JwkConstants.CurrentJwkCache, credentials, cacheEntryOptions);

            return credentials;
        }
        return credentials;
    }

    public async Task<ReadOnlyCollection<KeyMaterial>?> GetLastKeys(int quantity = 5)
    {
        if (!_memoryCache.TryGetValue(JwkConstants.JwksCache, out ReadOnlyCollection<KeyMaterial>? keys))
        {
            keys = _dbContext.SecurityKeys
                .OrderByDescending(x => x.CreationDate)
                .Take(quantity)
                .AsNoTrackingWithIdentityResolution()
                .ToList()
                .AsReadOnly();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_options.Value.CacheTime);

            if (keys.Count != 0)
                _memoryCache.Set(JwkConstants.JwksCache, keys, cacheEntryOptions);

            return keys;
        }
        return keys;
    }

    public async Task Revoke(KeyMaterial keyMaterial, string? reason = null)
    {
        if (keyMaterial is null) return;

        keyMaterial.Revoke(reason);
        _dbContext.Attach(keyMaterial);
        _dbContext.SecurityKeys.Update(keyMaterial);
        await _dbContext.SaveChangesAsync();
        ClearCache();
    }

    public async Task Store(KeyMaterial keyMaterial)
    {
        await _dbContext.SecurityKeys.AddAsync(keyMaterial);

        _logger.LogInformation("Storing key material with ID: {KeyId}", keyMaterial.KeyId);

        await _dbContext.SaveChangesAsync();

        ClearCache();
    }

    private void ClearCache()
    {
        _memoryCache.Remove(JwkConstants.JwksCache);
        _memoryCache.Remove(JwkConstants.CurrentJwkCache);
    }
}
