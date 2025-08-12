using Microsoft.IdentityModel.Tokens;

namespace Zypher.Security.JwtSigningCredentials.Interfaces;

public interface IJsonWebKeySetService
{
    SigningCredentials Generate(JwksOptions? options = null);
    SigningCredentials GetCurrent(JwksOptions? options = null);
    IReadOnlyCollection<JsonWebKey> GetLastKeysCredentials(int quantity);
}
