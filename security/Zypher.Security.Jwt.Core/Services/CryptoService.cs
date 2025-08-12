using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Zypher.Security.Jwt.Core.Jwa;

namespace Zypher.Security.Jwt.Core.Services;

internal static class CryptoService
{
    public static RsaSecurityKey CreateRsaSecurityKey(int keySize = 3072) => new(RSA.Create(keySize))
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

    internal static string GetCurveType(Algorithm algorithm) => algorithm.Alg switch
    {
        SecurityAlgorithms.EcdsaSha256 => JsonWebKeyECTypes.P256,
        SecurityAlgorithms.EcdsaSha384 => JsonWebKeyECTypes.P384,
        SecurityAlgorithms.EcdsaSha512 => JsonWebKeyECTypes.P521,
        _ => throw new NotSupportedException($"The algorithm '{algorithm.Alg}' does not support a curve type.")
    };

    internal static HMAC CreateHmacSecurityKey(Algorithm algorithm) => algorithm.Alg switch
    {
        DigitalSignaturesAlgorithm.HmacSha256 => new HMACSHA256(CreateRandomKey(64)),
        DigitalSignaturesAlgorithm.HmacSha384 => new HMACSHA384(CreateRandomKey(128)),
        DigitalSignaturesAlgorithm.HmacSha512 => new HMACSHA512(CreateRandomKey(128)),
        _ => throw new CryptographicException($"Could not create HMAC key based on algorithm {algorithm} (Could not parse expected SHA version)")
    };

    internal static Aes CreateAESSecurityKey(Algorithm algorithm)
    {
        var aesKey = Aes.Create();

        var aesKeySize = algorithm.Alg switch
        {
            EncryptionAlgorithmKey.Aes128KW => 128,
            EncryptionAlgorithmKey.Aes256KW => 256,
            _ => throw new NotSupportedException($"The algorithm '{algorithm.Alg}' does not support AES encryption.")
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
        RandomNumberGenerator.Fill(data);
        return data;
    }
}
