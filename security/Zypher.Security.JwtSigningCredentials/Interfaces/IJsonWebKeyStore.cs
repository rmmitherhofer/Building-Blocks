using Zypher.Security.JwtSigningCredentials.Models;

namespace Zypher.Security.JwtSigningCredentials.Interfaces;

public interface IJsonWebKeyStore
{
    void Save(SecurityKeyWithPrivate securityParameters);
    SecurityKeyWithPrivate GetCurrentKey();
    IReadOnlyCollection<SecurityKeyWithPrivate> Get(int quantity = 5);
    void Clear();
    bool NeedsUpdate();
    void Update(SecurityKeyWithPrivate securityKeyWithPrivate);
}
