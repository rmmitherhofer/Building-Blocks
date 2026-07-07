using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Zypher.Security.JwtSigningCredentials.Interfaces;
using Zypher.Security.JwtSigningCredentials.Models;

namespace Zypher.Security.JwtSigningCredentials.Jwks;

public class JwksService : IJsonWebKeySetService
{
    private readonly IJsonWebKeyStore _store;
    private readonly IOptions<JwksOptions> _options;
    private readonly IJsonWebKeyService _jwkService;

    public JwksService(IJsonWebKeyStore store, IOptions<JwksOptions> options, IJsonWebKeyService jwkService)
    {
        _store = store;
        _options = options;
        _jwkService = jwkService;
    }

    public SigningCredentials Generate(JwksOptions? options = null)
    {
        options ??= _options.Value;

        var key = _jwkService.Generate(options.Algorithm);

        SecurityKeyWithPrivate securityKeyWithPrivate = new();
        securityKeyWithPrivate.SetParameters(key, options.Algorithm);

        _store.Save(securityKeyWithPrivate);

        return new(key, options.Algorithm);
    }

    public SigningCredentials GetCurrent(JwksOptions? options = null)
    {
        if (!_store.NeedsUpdate())
        {
            RemovePrivateKeys();
            return Generate(options);
        }

        var currentKey = _store.GetCurrentKey();

        if (!CheckCompatibility(currentKey, options))
            currentKey = _store.GetCurrentKey();

        return currentKey.GetSigningCredentials();
    }

    public IReadOnlyCollection<JsonWebKey?> GetLastKeysCredentials(int quantity)
    {
        var store = _store.Get(quantity);
        if (store.Count == 0)
        {
            GetCurrent();

            return _store.Get(quantity)
                .OrderByDescending(x => x.CreationDate)
                .Select(x => x.GetSecurityKey())
                .ToList()
                .AsReadOnly();
        }

        return store.OrderByDescending(x => x.CreationDate)
            .Select(x => x.GetSecurityKey())
            .ToList()
            .AsReadOnly();
    }

    private void RemovePrivateKeys()
    {
        foreach (var securityKeyWithPrivate in _store.Get(_options.Value.AlgorithmsToKeep))
        {
            securityKeyWithPrivate.SetParameters();
            _store.Update(securityKeyWithPrivate);
        }
    }

    private bool CheckCompatibility(SecurityKeyWithPrivate currentKey, JwksOptions options)
    {
        options ??= _options.Value;

        if (currentKey is null)
        {
            Generate(options);
            return false;
        }

        if (currentKey.Algorithm == options.Algorithm) return true;

        Generate(options);

        return false;
    }
}
