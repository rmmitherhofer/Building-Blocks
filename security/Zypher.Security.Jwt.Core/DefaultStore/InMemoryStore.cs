using System.Collections.ObjectModel;
using Zypher.Security.Jwt.Core.Interfaces;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Security.Jwt.Core.DefaultStore;

internal class InMemoryStore : IJsonWebKeyStore
{
    internal const string DefaultRevocationReason = "Revoked";
    private static readonly List<KeyMaterial> _store = [];
    private readonly SemaphoreSlim _slim = new(1);

    public Task Clear()
    {
        _store.Clear();
        return Task.CompletedTask;
    }

    public Task<KeyMaterial> Get(string keyId) => Task.FromResult(_store.FirstOrDefault(x => x.KeyId == keyId));

    public Task<KeyMaterial?> GetCurrent() => Task.FromResult(_store.OrderByDescending(x => x.CreationDate).FirstOrDefault());

    public Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int quantity) => Task.FromResult(_store.OrderByDescending(x => x.CreationDate).Take(quantity).ToList().AsReadOnly());

    public async Task Revoke(KeyMaterial keyMaterial, string? reason = null)
    {
        if (keyMaterial is null) return;

        var revokeReason = reason ?? DefaultRevocationReason;

        keyMaterial.Revoke(revokeReason);

        var oldOne = _store.FirstOrDefault(x => x.Id == keyMaterial.Id);

        if (oldOne is not null)
        {
            var index = _store.FindIndex(x => x.Id == keyMaterial.Id);
            await _slim.WaitAsync();
            _store.Remove(oldOne);
            _store.Insert(index, keyMaterial);
            _slim.Release();
        }
    }

    public Task Store(KeyMaterial keyMaterial)
    {
        _slim.Wait();
        _store.Add(keyMaterial);
        _slim.Release();

        return Task.CompletedTask;
    }
}