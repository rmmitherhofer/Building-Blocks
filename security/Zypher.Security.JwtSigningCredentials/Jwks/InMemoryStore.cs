using Microsoft.Extensions.Options;
using Zypher.Security.JwtSigningCredentials.Interfaces;
using Zypher.Security.JwtSigningCredentials.Models;

namespace Zypher.Security.JwtSigningCredentials.Jwks;

internal class InMemoryStore : IJsonWebKeyStore
{
    private readonly IOptions<JwksOptions> _options;
    static readonly object lockObject = new();
    private List<SecurityKeyWithPrivate> _store;
    private SecurityKeyWithPrivate _current;

    public InMemoryStore(IOptions<JwksOptions> options)
    {
        _options = options;
        _store = [];
    }
    public void Clear() => _store.Clear();
    public IReadOnlyCollection<SecurityKeyWithPrivate> Get(int quantity = 5) => _store.OrderByDescending(x => x.CreationDate)
        .Take(quantity)
        .ToList()
        .AsReadOnly();

    public SecurityKeyWithPrivate GetCurrentKey() => _current;

    public bool NeedsUpdate()
    {
        if (_current is null) return true;

        return _current.CreationDate.AddDays(_options.Value.DaysUntilExpire) < DateTime.UtcNow.Date;
    }

    public void Save(SecurityKeyWithPrivate securityParameters)
    {
        lock (lockObject)
            _store.Add(securityParameters);

        _current = securityParameters;
    }

    public void Update(SecurityKeyWithPrivate securityKeyWithPrivate)
    {
        var oldOne = _store.Find(x => x.Id == securityKeyWithPrivate.Id);

        if (oldOne is null) return;

        var index = _store.FindIndex(x => x.Id == securityKeyWithPrivate.Id);

        Monitor.Enter(lockObject);
        _store.RemoveAt(index);
        _store.Insert(index, securityKeyWithPrivate);
        Monitor.Exit(lockObject);
    }
}
