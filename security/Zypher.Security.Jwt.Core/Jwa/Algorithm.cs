using Microsoft.IdentityModel.Tokens;

namespace Zypher.Security.Jwt.Core.Jwa;

public class Algorithm
{
    public EncryptionAlgorithmContent EncryptionAlgorithmContent { get; set; }
    public AlgorithmType AlgorithmType { get; internal set; }
    public CryptographyType CryptographyType { get; internal set; }
    public JwtType JwtType => CryptographyType == CryptographyType.Encryption ? JwtType.Jwe : JwtType.Jws;
    public string Alg { get; internal set; }
    public string Curve { get; set; }

    public Algorithm(string algorithm)
    {
        switch (algorithm)
        {
            case EncryptionAlgorithmKey.Aes128KW:
            case EncryptionAlgorithmKey.Aes256KW:
                AlgorithmType = AlgorithmType.AES;
                CryptographyType = CryptographyType.Encryption;
                break;
            case EncryptionAlgorithmKey.RsaPKCS1:
            case EncryptionAlgorithmKey.RsaOAEP:
                CryptographyType = CryptographyType.Encryption;
                AlgorithmType = AlgorithmType.RSA;
                break;
            case DigitalSignaturesAlgorithm.EcdsaSha256:
            case DigitalSignaturesAlgorithm.EcdsaSha384:
            case DigitalSignaturesAlgorithm.EcdsaSha512:
                CryptographyType = CryptographyType.DigitalSignature;
                AlgorithmType = AlgorithmType.ECDsa;
                break;
            case DigitalSignaturesAlgorithm.HmacSha256:
            case DigitalSignaturesAlgorithm.HmacSha384:
            case DigitalSignaturesAlgorithm.HmacSha512:
                CryptographyType = CryptographyType.DigitalSignature;
                AlgorithmType = AlgorithmType.HMAC;
                break;
            case DigitalSignaturesAlgorithm.RsaSha256:
            case DigitalSignaturesAlgorithm.RsaSha384:
            case DigitalSignaturesAlgorithm.RsaSha512:
            case DigitalSignaturesAlgorithm.RsaSsaPssSha256:
            case DigitalSignaturesAlgorithm.RsaSsaPssSha384:
            case DigitalSignaturesAlgorithm.RsaSsaPssSha512:
                CryptographyType = CryptographyType.DigitalSignature;
                AlgorithmType = AlgorithmType.RSA;
                break;
            default:
                throw new NotSupportedException($"The algorithm '{algorithm}' is not supported.");
        }
        Alg = algorithm;
    }
    public Algorithm() => AlgorithmType = AlgorithmType.RSA;

    public Algorithm WithCurve(string curve)
    {
        if (AlgorithmType != AlgorithmType.ECDsa)
            throw new InvalidOperationException("Only Elliptic Curves accept curves");

        Curve = curve;
        return this;
    }

    public Algorithm WithContentEncryption(EncryptionAlgorithmContent enc)
    {
        if (CryptographyType == CryptographyType.DigitalSignature)
            throw new InvalidOperationException("Only Json Web Encryption has enc param");

        EncryptionAlgorithmContent = (string)enc switch
        {
            EncryptionAlgorithmContent.Aes128CbcHmacSha256
            or EncryptionAlgorithmContent.Aes128Gcm
            or EncryptionAlgorithmContent.Aes192CbcHmacSha384
            or EncryptionAlgorithmContent.Aes192Gcm
            or EncryptionAlgorithmContent.Aes256CbcHmacSha512
            or EncryptionAlgorithmContent.Aes256Gcm => enc,
            _ => throw new NotSupportedException($"The encryption algorithm '{enc}' is not supported."),
        };
        return this;
    }

    public string Kty() => AlgorithmType switch
    {
        AlgorithmType.RSA => JsonWebAlgorithmsKeyTypes.RSA,
        AlgorithmType.ECDsa => JsonWebAlgorithmsKeyTypes.EllipticCurve,
        AlgorithmType.HMAC => JsonWebAlgorithmsKeyTypes.Octet,
        AlgorithmType.AES => JsonWebAlgorithmsKeyTypes.Octet,
        _ => throw new ArgumentOutOfRangeException($"The algorithm type '{AlgorithmType}' is not supported.")
    };

    public static Algorithm Create(string algorithm) => new(algorithm);

    public static Algorithm Create(AlgorithmType algorithmType, JwtType jwtType)
    {
        if (jwtType == JwtType.Both) return new();

        if(jwtType == JwtType.Jws) return algorithmType switch
        {
            AlgorithmType.RSA => new(DigitalSignaturesAlgorithm.RsaSsaPssSha256),
            AlgorithmType.ECDsa => new Algorithm(DigitalSignaturesAlgorithm.EcdsaSha256).WithCurve(JsonWebKeyECTypes.P256),
            AlgorithmType.HMAC => new(DigitalSignaturesAlgorithm.HmacSha256),
            _ => throw new NotSupportedException($"Invalid algorithm for json web signature (JWS): {algorithmType}")
        };

        return algorithmType switch
        {
            AlgorithmType.RSA => new Algorithm(EncryptionAlgorithmKey.RsaOAEP).WithContentEncryption(EncryptionAlgorithmContent.Aes128CbcHmacSha256),
            AlgorithmType.AES => new Algorithm (EncryptionAlgorithmKey.Aes128KW).WithContentEncryption(EncryptionAlgorithmContent.Aes128CbcHmacSha256),
            _ => throw new NotSupportedException($"Invalid algorithm for json web encryption (JWE): {algorithmType}")
        };
    }

    public static implicit operator string(Algorithm value) => value.Alg;
    public static implicit operator Algorithm(string value) => new(value);
}
