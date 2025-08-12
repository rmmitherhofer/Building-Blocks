using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

namespace Zypher.Security.JwtSigningCredentials.Models;

public class PublicJsonWebKey
{
    [JsonPropertyName("kty")]
    public string KeyType { get; set; }
    [JsonPropertyName("use")]
    public string PublicKeyUse { get; set; }
    [JsonPropertyName("key_ops")]
    public IList<string> KeyOperations { get; set; }

    [JsonPropertyName("alg")]
    public string Algorithm { get; set; }

    [JsonPropertyName("kid")]
    public string KeyId { get; set; }

    [JsonPropertyName("x5u")]
    public string X509Url { get; set; }

    [JsonPropertyName("x5c")]
    public IList<string> X509Chain { get; set; }

    [JsonPropertyName("x5t")]
    public string X5tS256 { get; set; }

    [JsonPropertyName("crv")]
    public string CurveName { get; set; }

    [JsonPropertyName("x")]
    public string X { get; set; }

    [JsonPropertyName("y")]
    public string Y { get; set; }
    [JsonPropertyName("n")]
    public string Modulus { get; set; }
    [JsonPropertyName("e")]
    public string Exponent { get; set; }
    [JsonPropertyName("k")]
    public string Key { get; set; }

    public PublicJsonWebKey(JsonWebKey jwk)
    {
        KeyType = jwk.Kty;
        PublicKeyUse = jwk.Use ?? "sig";
        KeyId = jwk.Kid;
        Algorithm = jwk.Alg;

        if (jwk.KeyOps.Any())
            KeyOperations = jwk.KeyOps;

        if (jwk.X5c.Any())
            X509Chain = jwk.X5c;

        X509Url = jwk.X5u;
        X5tS256 = jwk.X5t;

        if (jwk.Kty == JsonWebAlgorithmsKeyTypes.EllipticCurve)
        {
            CurveName = jwk.Crv;
            X = jwk.X;
            Y = jwk.Y;
        }

        if (jwk.Kty == JsonWebAlgorithmsKeyTypes.RSA)
        {
            Modulus = jwk.N;
            Exponent = jwk.E;
        }

        if (jwk.Kty == JsonWebAlgorithmsKeyTypes.Octet)
        {
            Key = jwk.K;
        }
    }

    public static PublicJsonWebKey FromJwk(JsonWebKey jwk) => new(jwk);
}
