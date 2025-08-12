using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zypher.Security.Jwt.Core.Models;

public class KeyMaterial
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string KeyId { get; set; }
    public string Type { get; set; }
    public string Parameters { get; set; }
    public bool IsRevoked { get; set; }
    public string? RevokedReason { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ExpiredAt { get; set; }

    public KeyMaterial() { }

    public KeyMaterial(CryptographicKey cryptographicKey)
    {
        CreationDate = DateTime.UtcNow;
        Parameters = JsonSerializer.Serialize(cryptographicKey.GetJsonWebKey(), typeof(JsonWebKey));
        Type = cryptographicKey.Algorithm.Kty();
        KeyId = cryptographicKey.Key.KeyId;
    }

    public JsonWebKey GetSecurityKey() => JsonWebKey.Create(Parameters);
    public void Revoke(string? reason = null)
    {
        IsRevoked = true;
        RevokedReason = reason;
        ExpiredAt = DateTime.UtcNow;
        Parameters = JsonSerializer.Serialize(PublicJsonWebKey.FromJwk(GetSecurityKey()), new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        });
    }
    public bool IsExpired(int daysUntilExpire) => CreationDate.AddDays(daysUntilExpire) < DateTime.UtcNow.Date;

    public static implicit operator SecurityKey(KeyMaterial value) => value.GetSecurityKey();
}
