using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;
using Zypher.Security.Jwt.Core.Interfaces;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Security.Jwt.Core.Jwt;

internal class JwtService : IJwtService
{
    private readonly IJsonWebKeyStore _store;
    private readonly IOptions<JwtOptions> _options;

    public JwtService(IJsonWebKeyStore store, IOptions<JwtOptions> options)
    {
        _store = store;
        _options = options;
    }

    public async Task<SecurityKey> GenerateKey()
    {
        var model = new KeyMaterial(new (_options.Value.Jws));

        await _store.Store(model);

        return model.GetSecurityKey();
    }

    public async Task<SecurityKey> GenerateNewKey()
    {
        await _store.Revoke(await _store.GetCurrent());
        return await GenerateKey();
    }

    public async Task<EncryptingCredentials> GetCurrentEncryptingCredentials() => new(await GetCurrentSecurityKey(), _options.Value.Jwe.Alg, _options.Value.Jwe.EncryptionAlgorithmContent);

    public async Task<SecurityKey> GetCurrentSecurityKey()
    {
        var current = await _store.GetCurrent();

        if (NeedsUpdate(current))
        {
            await _store.Revoke(current);

            var newKey = await GenerateKey();

            return newKey;
        }

        if (!await CheckCompatibility(current))
            current = await _store.GetCurrent();

        return current;
    }

    public async Task<SigningCredentials> GetCurrentSigningCredentials()
    {
        var current = await GetCurrentSecurityKey();

        return new(current, _options.Value.Jws);
    }

    public async Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int? i = null) => await _store.GetLastKeys(_options.Value.AlgorithmToKeep);

    public async Task RevokeKey(string keyId, string? reason = null) => await _store.Revoke(await _store.Get(keyId), reason);

    private async Task<bool> CheckCompatibility(KeyMaterial? current)
    {
        if (current.Type != _options.Value.Jws.Kty())
        {
            await GenerateKey();
            return false;
        }
        return true;
    }

    private bool NeedsUpdate(KeyMaterial? current) => current?.IsExpired(_options.Value.DaysUntilExpire) != false || current.IsRevoked;

}
