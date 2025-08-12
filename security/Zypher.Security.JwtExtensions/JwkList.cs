using Microsoft.IdentityModel.Tokens;

namespace Zypher.Security.JwtExtensions;

public class JwkList
{
    public DateTime When { get; set; }
    public JsonWebKeySet Jwks { get; set; }

    public JwkList(JsonWebKeySet jwks)
    {
        Jwks = jwks;
        When = DateTime.UtcNow;
    }
}
