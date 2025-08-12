using System.Collections.ObjectModel;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Security.Jwt.Core.Interfaces;

public interface IJsonWebKeyStore
{
    Task Store(KeyMaterial keyMaterial);
    Task<KeyMaterial> GetCurrent();
    Task Revoke(KeyMaterial keyMaterial, string? reason = null);
    Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int quantity);
    Task<KeyMaterial> Get(string keyId);
    Task Clear();
}
