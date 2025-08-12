using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Zypher.Security.JwtSigningCredentials.Jwk;

internal static class CryptoService
{
    public static RsaSecurityKey CreateRsaSecurityKey(int keySize = 2048) => new(RSA.Create(keySize))
    {
        KeyId = CreateUniqueId()
    };

    public static string CreateUniqueId(int length = 16) => Base64UrlEncoder.Encode(CreateRandomKey(length));

    internal static ECDsaSecurityKey CreateEcdsaSecurityKey(Algorithm algorithm)
    {
        var curve = algorithm.Curve;

        if (string.IsNullOrEmpty(algorithm.Curve)) curve = GetCurveType(algorithm);

        return new(ECDsa.Create(GetNamedECCurve(curve)))
        {
            KeyId = CreateUniqueId()
        };
    }

    internal static string GetCurveType(Algorithm algorithm) => algorithm.Selected switch
    {
        SecurityAlgorithms.EcdsaSha256 => JsonWebKeyECTypes.P256,
        SecurityAlgorithms.EcdsaSha384 => JsonWebKeyECTypes.P384,
        SecurityAlgorithms.EcdsaSha512 => JsonWebKeyECTypes.P521,
        _ => throw new NotSupportedException($"The algorithm '{algorithm.Selected}' does not support a curve type.")
    };

    internal static HMAC CreateHmacSecurityKey(Algorithm algorithm) => algorithm.Selected switch
    {
        SecurityAlgorithms.HmacSha256 => new HMACSHA256(CreateRandomKey(64)),
        SecurityAlgorithms.HmacSha384 => new HMACSHA384(CreateRandomKey(128)),
        SecurityAlgorithms.HmacSha512 => new HMACSHA512(CreateRandomKey(128)),
        _ => throw new CryptographicException($"Could not create HMAC key based on algorithm {algorithm} (Could not parse expected SHA version)")
    };

    internal static Aes CreateAESSecurityKey(Algorithm algorithm)
    {
        var aesKey = Aes.Create();

        var aesKeySize = algorithm.Selected switch
        {
            SecurityAlgorithms.Aes128KW => 128,
            SecurityAlgorithms.Aes256KW => 256,
            _ => throw new NotSupportedException($"The algorithm '{algorithm.Selected}' does not support AES encryption.")
        };

        aesKey.KeySize = aesKeySize;

        aesKey.GenerateKey();

        return aesKey;
    }

    internal static ECCurve GetNamedECCurve(string curveId)
    {
        if (string.IsNullOrEmpty(curveId))
            throw LogHelper.LogArgumentNullException(nameof(curveId));

        return curveId switch
        {
            JsonWebKeyECTypes.P256 => ECCurve.NamedCurves.nistP256,
            JsonWebKeyECTypes.P384 => ECCurve.NamedCurves.nistP384,
            JsonWebKeyECTypes.P512 => ECCurve.NamedCurves.nistP521,
            JsonWebKeyECTypes.P521 => ECCurve.NamedCurves.nistP521,
            _ => throw LogHelper.LogExceptionMessage(new ArgumentException(curveId))
        };
    }

    internal static byte[] CreateRandomKey(int length)
    {
        byte[] data = new byte[length];
        var rng = RandomNumberGenerator.Create();

        rng.GetBytes(data);

        return data;
    }
}
