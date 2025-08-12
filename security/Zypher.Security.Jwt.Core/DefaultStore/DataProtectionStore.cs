using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Xml.Linq;
using Zypher.Security.Jwt.Core.Interfaces;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Security.Jwt.Core.DefaultStore;

internal class DataProtectionStore : IJsonWebKeyStore
{
    internal static readonly XName IdAttributeName = "id";
    internal static readonly XName VersionAttributeName = "version";
    internal static readonly XName CreationDateElementName = "creationName";
    internal static readonly XName ActivationDateElementName = "activationName";
    internal static readonly XName ExpirationDateElementName = "expirationName";
    internal static readonly XName DescriptorElementName = "descriptor";
    internal static readonly XName DeserializerTypeAttributeName = "deserializerType";
    internal static readonly XName RevocationElementName = "ZypherSecurityJwtRevocation";
    internal static readonly XName RevocationDateElementName = "revocationDate";
    internal static readonly XName ReasonElementName = "reason";


    private readonly ILoggerFactory _loggerFactory;
    private readonly IOptions<JwtOptions> _options;
    private readonly IOptions<KeyManagementOptions> _keyManagementOptions;
    private readonly IMemoryCache _memoryCache;
    private readonly IDataProtector _dataProtector;
    private IXmlRepository KeyRepository => _keyManagementOptions.Value.XmlRepository ?? GetFallbackKeyRepositoryEncryptorPair();

    private const string Name = "ZypherSecurityJwt";
    internal const string DefaultRevocationReason = "Revoked";

    public DataProtectionStore(
        ILoggerFactory loggerFactory,
        IOptions<JwtOptions> options,
        IOptions<KeyManagementOptions> keyManagementOptions,
        IMemoryCache memoryCache,
        IDataProtectionProvider provider)
    {
        _loggerFactory = loggerFactory;
        _options = options;
        _keyManagementOptions = keyManagementOptions;
        _memoryCache = memoryCache;
        _dataProtector = provider.CreateProtector(nameof(KeyMaterial));
    }

    public async Task Clear()
    {
        foreach (var securityKeyWithPrivate in GetKeys())
            await Revoke(securityKeyWithPrivate);
    }

    public Task<KeyMaterial> Get(string keyId)
        => Task.FromResult(GetKeys().FirstOrDefault(x => x.KeyId.Equals(keyId, StringComparison.OrdinalIgnoreCase)));

    public async Task<KeyMaterial?> GetCurrent()
    {
        if (!_memoryCache.TryGetValue(JwkConstants.CurrentJwkCache, out KeyMaterial? keyMaterial))
        {
            var keys = await GetLastKeys(1);

            keyMaterial = keys.FirstOrDefault();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_options.Value.CacheTime);

            if (keyMaterial is not null)
                _memoryCache.Set(JwkConstants.CurrentJwkCache, keyMaterial, cacheEntryOptions);
        }
        return keyMaterial;
    }

    public Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int quantity = 5)
    {
        if (!_memoryCache.TryGetValue(JwkConstants.JwksCache, out IReadOnlyCollection<KeyMaterial> keys))
        {
            keys = GetKeys();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_options.Value.CacheTime);

            if (keys.Count != 0)
                _memoryCache.Set(JwkConstants.JwksCache, keys, cacheEntryOptions);
        }

        return Task.FromResult(keys!
            .OrderByDescending(x => x.CreationDate)
            .ToList()
            .AsReadOnly());
    }

    public async Task Revoke(KeyMaterial keyMaterial, string? reason = null)
    {
        if (keyMaterial is null) return;

        var keys = await GetLastKeys();

        var key = keys.First(x => x.Id == keyMaterial.Id);

        if (key is { IsRevoked: true }) return;

        keyMaterial.Revoke();

        var revokeReason = reason ?? DefaultRevocationReason;

        var revocationElement = new XElement(RevocationElementName,
            new XAttribute(VersionAttributeName, 1),
            new XElement(RevocationDateElementName, DateTimeOffset.UtcNow),
            new XElement(Name, new XAttribute(IdAttributeName, keyMaterial.Id),
            new XElement(ReasonElementName, revokeReason)));

        var friendlyName = string.Format(CultureInfo.InvariantCulture, "revocation-{0}-{1:D}-{2:yyyy_MM_dd_hh_mm_fffffff}", keyMaterial.Type, keyMaterial.Id, DateTime.UtcNow);
        KeyRepository.StoreElement(revocationElement, friendlyName);

        ClearCache();
    }

    public Task Store(KeyMaterial keyMaterial)
    {
        var possiblyEncryptedKeyElement = _dataProtector.Protect(JsonSerializer.Serialize(keyMaterial));

        var keyElement = new XElement(Name,
            new XAttribute(IdAttributeName, keyMaterial.Id),
            new XAttribute(VersionAttributeName, 1),
            new XElement(CreationDateElementName, DateTimeOffset.UtcNow),
            new XElement(ActivationDateElementName, DateTimeOffset.UtcNow),
            new XElement(ExpirationDateElementName, DateTimeOffset.UtcNow.AddDays(_options.Value.DaysUntilExpire)),
            new XElement(DescriptorElementName,
                new XAttribute(DeserializerTypeAttributeName, typeof(KeyMaterial).AssemblyQualifiedName!),
                possiblyEncryptedKeyElement));

        var friendlyName = string.Format(CultureInfo.InvariantCulture, "key-{0}", keyMaterial.KeyId);
        KeyRepository.StoreElement(keyElement, friendlyName);
        ClearCache();

        return Task.CompletedTask;
    }

    private void ClearCache()
    {
        _memoryCache.Remove(JwkConstants.JwksCache);
        _memoryCache.Remove(JwkConstants.CurrentJwkCache);
    }
    private IReadOnlyCollection<KeyMaterial> GetKeys()
    {
        List<KeyMaterial> keys = [];
        List<RevokedKeyInfo> revokedKeys = [];

        foreach (var element in KeyRepository.GetAllElements())
        {
            if (element.Name == Name)
            {
                var descriptorElement = element.Element(DescriptorElementName);
                var expectedDescriptorType = typeof(KeyMaterial).FullName;
                var descriptorType = descriptorElement?.Attribute(DeserializerTypeAttributeName);

                if (descriptorType?.Value.Contains(expectedDescriptorType) != true) continue;

                string? unecryptedInputToDeserialize = null;

                try
                {
                    unecryptedInputToDeserialize = _dataProtector.Unprotect(descriptorElement.Value);
                }
                catch
                {
                    continue;
                }

                var key = JsonSerializer.Deserialize<KeyMaterial>(unecryptedInputToDeserialize);

                if (key.IsExpired(_options.Value.DaysUntilExpire))
                    revokedKeys.Add(new RevokedKeyInfo(key.Id.ToString()));

                keys.Add(key);
            }
            else if (element.Name == RevocationElementName)
            {
                var keyIdAsString = (string)element.Element(Name)!.Attribute(IdAttributeName)!;
                var reason = (string)element.Element(ReasonElementName);

                revokedKeys.Add(new RevokedKeyInfo(keyIdAsString, reason));
            }
        }

        foreach (var revokedKey in revokedKeys)
            keys.FirstOrDefault(x => x.Id.ToString().Equals(revokedKey.Id))?.Revoke(revokedKey.RevokedReason);

        return [.. keys];
    }
    internal IXmlRepository GetFallbackKeyRepositoryEncryptorPair()
    {
        IXmlRepository key;

        var forAzureWebSites = DefaultKeyStorageDirectories.Instace.GetKeyStorageDirectoryForAzureWebSites();
        if (forAzureWebSites is not null)
        {
            key = new FileSystemXmlRepository(forAzureWebSites, _loggerFactory);
        }
        else
        {
            RegistryKey? registryKey = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                registryKey = RegistryXmlRepository.DefaultRegistryKey;

            if (registryKey is not null)
                key = new RegistryXmlRepository(RegistryXmlRepository.DefaultRegistryKey, _loggerFactory);

            else
                throw new Exception("No valid key storage directory found. Please ensure that the environment is set up correctly for key storage.");

        }
        return key;
    }
}
