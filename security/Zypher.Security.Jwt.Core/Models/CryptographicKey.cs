using Microsoft.IdentityModel.Tokens;
using Zypher.Security.Jwt.Core.Jwa;
using Zypher.Security.Jwt.Core.Services;

namespace Zypher.Security.Jwt.Core.Models;

public class CryptographicKey
{
    public Algorithm Algorithm { get; set; }

    public SecurityKey Key { get; set; }

    public CryptographicKey(Algorithm algorithm)
    {
        Algorithm = algorithm;
        Key = algorithm.AlgorithmType switch
        {
            AlgorithmType.RSA => GenerateRsa(),
            AlgorithmType.ECDsa => GenerateECDsa(algorithm),
            AlgorithmType.HMAC => GenerateHMAC(algorithm),
            AlgorithmType.AES => GenerateAES(algorithm),
            _ => throw new ArgumentOutOfRangeException(nameof(AlgorithmType), algorithm.AlgorithmType, $"Algorithm type {algorithm.AlgorithmType} is not supported.")
        };
    }

    public JsonWebKey GetJsonWebKey() => Algorithm.AlgorithmType switch
    {
        AlgorithmType.RSA => JsonWebKeyConverter.ConvertFromRSASecurityKey((RsaSecurityKey)Key),
        AlgorithmType.ECDsa => JsonWebKeyConverter.ConvertFromECDsaSecurityKey((ECDsaSecurityKey)Key),
        AlgorithmType.HMAC => JsonWebKeyConverter.ConvertFromSymmetricSecurityKey((SymmetricSecurityKey)Key),
        AlgorithmType.AES => JsonWebKeyConverter.ConvertFromSymmetricSecurityKey((SymmetricSecurityKey)Key),
        _ => throw new ArgumentOutOfRangeException($"Algorithm type {Algorithm.AlgorithmType} is not supported.")
    };

    private SecurityKey GenerateRsa() => CryptoService.CreateRsaSecurityKey();
    private SecurityKey GenerateECDsa(Algorithm algorithm) => CryptoService.CreateEcdsaSecurityKey(algorithm);
    private SecurityKey GenerateHMAC(Algorithm algorithm)
    {
        var key = CryptoService.CreateHmacSecurityKey(algorithm);
        return new SymmetricSecurityKey(key.Key)
        {
            KeyId = CryptoService.CreateUniqueId()
        };
    }
    private SecurityKey GenerateAES(Algorithm algorithm)
    {
        var aesKey = CryptoService.CreateAESSecurityKey(algorithm);

        return new SymmetricSecurityKey(aesKey.Key)
        {
            KeyId = CryptoService.CreateUniqueId()
        };
    }

    public static implicit operator SecurityKey(CryptographicKey value) => value.Key;
}
