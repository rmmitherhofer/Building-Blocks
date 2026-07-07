using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Zypher.Security.Jwt.Core.Jwa;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Security.Jwt.Core.Tests.Models;

public class CryptographicKeyTests
{
    [Fact(DisplayName =
        "Given a RSA algorithm, " +
        "When CryptographicKey is created, " +
        "Then it creates an RSA key")]
    [Trait("Type", nameof(CryptographicKey))]
    public async Task CryptographicKey_Rsa_CreatesKey()
    {
        //Given
        var algorithm = new Algorithm(DigitalSignaturesAlgorithm.RsaSha256);

        //When
        var key = new CryptographicKey(algorithm);

        //Then
        key.Key.Should().BeOfType<RsaSecurityKey>();
        key.Key.KeyId.Should().NotBeNullOrEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an ECDsa algorithm, " +
        "When CryptographicKey is created, " +
        "Then it creates an ECDsa key")]
    [Trait("Type", nameof(CryptographicKey))]
    public async Task CryptographicKey_Ecdsa_CreatesKey()
    {
        //Given
        var algorithm = new Algorithm(DigitalSignaturesAlgorithm.EcdsaSha256).WithCurve(JsonWebKeyECTypes.P256);

        //When
        var key = new CryptographicKey(algorithm);

        //Then
        key.Key.Should().BeOfType<ECDsaSecurityKey>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an HMAC algorithm, " +
        "When CryptographicKey is created, " +
        "Then it creates a symmetric key")]
    [Trait("Type", nameof(CryptographicKey))]
    public async Task CryptographicKey_Hmac_CreatesKey()
    {
        //Given
        var algorithm = new Algorithm(DigitalSignaturesAlgorithm.HmacSha256);

        //When
        var key = new CryptographicKey(algorithm);

        //Then
        key.Key.Should().BeOfType<SymmetricSecurityKey>();
        key.Key.KeyId.Should().NotBeNullOrEmpty();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an AES algorithm, " +
        "When CryptographicKey is created, " +
        "Then it creates a symmetric key")]
    [Trait("Type", nameof(CryptographicKey))]
    public async Task CryptographicKey_Aes_CreatesKey()
    {
        //Given
        var algorithm = Algorithm.Create(AlgorithmType.AES, JwtType.Jwe);

        //When
        var key = new CryptographicKey(algorithm);

        //Then
        key.Key.Should().BeOfType<SymmetricSecurityKey>();
        key.Key.KeyId.Should().NotBeNullOrEmpty();
        await Task.CompletedTask;
    }
}
