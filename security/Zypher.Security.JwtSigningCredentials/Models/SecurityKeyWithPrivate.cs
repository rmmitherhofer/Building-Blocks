using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zypher.Security.JwtSigningCredentials.Models;

public class SecurityKeyWithPrivate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string KeyId { get; set; }
    public string Type { get; set; }
    public string Parameters { get; set; }
    public string Algorithm { get; set; }
    public DateTime CreationDate { get; set; }

    public void SetParameters(SecurityKey key, Algorithm algorithm)
    {
        KeyId = key.KeyId;
        Type = algorithm.Kty();
        Parameters = JsonSerializer.Serialize(key, typeof(JsonWebKey), new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
        Algorithm = algorithm;
        CreationDate = DateTime.UtcNow;
    }

    public JsonWebKey? GetSecurityKey() => JsonSerializer.Deserialize<JsonWebKey>(Parameters);

    public SigningCredentials GetSigningCredentials() => new(GetSecurityKey(), Algorithm);

    public void SetParameters()
    {
        var jwk = GetSecurityKey();

        Parameters = JsonSerializer.Serialize(PublicJsonWebKey.FromJwk(jwk), new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
    }
}