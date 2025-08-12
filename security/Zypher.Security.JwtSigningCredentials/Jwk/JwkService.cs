using Microsoft.IdentityModel.Tokens;
using Zypher.Security.JwtSigningCredentials.Interfaces;

namespace Zypher.Security.JwtSigningCredentials.Jwk;

public class JwkService : IJsonWebKeyService
{
    public JsonWebKey Generate(Algorithm algotithm) => algotithm.KeyType switch
    {
        KeyType.RSA => GenerateRsa(),
        KeyType.ECDsa => GenerateECDsa(algotithm),
        KeyType.HMAC => GenerateHMAC(algotithm),
        KeyType.AES => GenerateAES(algotithm),
        _ => throw new ArgumentOutOfRangeException(nameof(algotithm.KeyType), "Unsupported key type")
    };
    private JsonWebKey GenerateRsa() => JsonWebKeyConverter.ConvertFromRSASecurityKey(CryptoService.CreateRsaSecurityKey());

    private JsonWebKey GenerateECDsa(Algorithm algorithm)
    {
        var key = CryptoService.CreateEcdsaSecurityKey(algorithm);
        var parameters = key.ECDsa.ExportParameters(true);
        return new JsonWebKey
        {
            Kty = JsonWebAlgorithmsKeyTypes.EllipticCurve,
            Use = "sig",
            Kid = key.KeyId,
            KeyId = key.KeyId,
            X = Base64UrlEncoder.Encode(parameters.Q.X),
            Y = Base64UrlEncoder.Encode(parameters.Q.Y),
            D = Base64UrlEncoder.Encode(parameters.D),
            Crv = CryptoService.GetCurveType(algorithm),
            Alg = algorithm
        };
    }
    private JsonWebKey GenerateHMAC(Algorithm algorithm)
    {
        var key = CryptoService.CreateHmacSecurityKey(algorithm);
        var jwk = JsonWebKeyConverter.ConvertFromSymmetricSecurityKey(new SymmetricSecurityKey(key.Key));

        jwk.KeyId = CryptoService.CreateUniqueId();

        return jwk;
    }

    private JsonWebKey GenerateAES(Algorithm algorithm) => JsonWebKeyConverter.ConvertFromSymmetricSecurityKey(new SymmetricSecurityKey(CryptoService.CreateAESSecurityKey(algorithm).Key));

    public SigningCredentials GenerateSigningCredentials(Algorithm algorithm) => new(Generate(algorithm), algorithm);

    public SigningCredentials GenereteSigningCredentials(SecurityKey key, Algorithm algorithm)
    {
        if(key is null)
            throw new ArgumentNullException(nameof(key), "SecurityKey cannot be null.");

        return new(key, algorithm);
    }
}