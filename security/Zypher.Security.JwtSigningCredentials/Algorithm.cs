using Microsoft.IdentityModel.Tokens;
using Zypher.Security.JwtSigningCredentials.Jwk;

namespace Zypher.Security.JwtSigningCredentials;

public sealed class Algorithm
{
    public static readonly Algorithm HS256 = new(SecurityAlgorithms.HmacSha256, KeyType.HMAC);
    public static readonly Algorithm HS384 = new(SecurityAlgorithms.HmacSha384, KeyType.HMAC);
    public static readonly Algorithm HS512 = new(SecurityAlgorithms.HmacSha512, KeyType.HMAC);

    public static readonly Algorithm RS256 = new(SecurityAlgorithms.RsaSha256, KeyType.RSA);
    public static readonly Algorithm RS384 = new(SecurityAlgorithms.RsaSha384, KeyType.RSA);
    public static readonly Algorithm RS512 = new(SecurityAlgorithms.RsaSha512, KeyType.RSA);
    public static readonly Algorithm PS256 = new(SecurityAlgorithms.RsaSsaPssSha256, KeyType.RSA);
    public static readonly Algorithm PS384 = new(SecurityAlgorithms.RsaSsaPssSha384, KeyType.RSA);
    public static readonly Algorithm PS512 = new(SecurityAlgorithms.RsaSsaPssSha512, KeyType.RSA);

    public static readonly Algorithm ES256 = new(SecurityAlgorithms.EcdsaSha256, KeyType.ECDsa, JsonWebKeyECTypes.P256);
    public static readonly Algorithm ES384 = new(SecurityAlgorithms.EcdsaSha384, KeyType.ECDsa, JsonWebKeyECTypes.P384);
    public static readonly Algorithm ES512 = new(SecurityAlgorithms.EcdsaSha512, KeyType.ECDsa, JsonWebKeyECTypes.P521);



    public KeyType KeyType { get; }
    public string Curve { get; }
    public string Selected { get; }

    public Algorithm(string selected, KeyType keyType, string curve)
    {
        KeyType = keyType;
        Curve = curve;
        Selected = selected;
    }

    public Algorithm(string selected, KeyType keyType)
    {
        KeyType = keyType;
        Selected = selected;
    }
    public static Algorithm Create(string selected, KeyType keyType) => new(selected, keyType);

    public string Kty() => KeyType switch
    {
        KeyType.RSA => JsonWebAlgorithmsKeyTypes.RSA,
        KeyType.ECDsa => JsonWebAlgorithmsKeyTypes.EllipticCurve,
        KeyType.HMAC => JsonWebAlgorithmsKeyTypes.Octet,
        KeyType.AES => JsonWebAlgorithmsKeyTypes.Octet,
        _ => throw new ArgumentOutOfRangeException(nameof(KeyType), "Unsupported key type")
    };

    public static implicit operator string(Algorithm value) => value.Selected;
}
