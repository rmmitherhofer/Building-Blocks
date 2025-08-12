using Microsoft.IdentityModel.Tokens;

namespace Zypher.Security.JwtSigningCredentials.Interfaces;

public interface IJsonWebKeyService
{
    JsonWebKey Generate(Algorithm algotithm);
}
