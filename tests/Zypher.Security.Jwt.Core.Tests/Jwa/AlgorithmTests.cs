using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using System;
using Xunit;
using Zypher.Security.Jwt.Core.Jwa;

namespace Zypher.Security.Jwt.Core.Tests.Jwa;

public class AlgorithmTests
{
    [Fact(DisplayName =
        "Given a known RSA algorithm, " +
        "When Algorithm is created, " +
        "Then it sets type and cryptography")]
    [Trait("Type", nameof(Algorithm))]
    public async Task Algorithm_Rsa_SetsTypes()
    {
        //Given
        var algorithm = new Algorithm(DigitalSignaturesAlgorithm.RsaSha256);

        //When
        var type = algorithm.AlgorithmType;

        //Then
        type.Should().Be(AlgorithmType.RSA);
        algorithm.CryptographyType.Should().Be(CryptographyType.DigitalSignature);
        algorithm.Alg.Should().Be(DigitalSignaturesAlgorithm.RsaSha256);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an unsupported algorithm, " +
        "When Algorithm is created, " +
        "Then it throws NotSupportedException")]
    [Trait("Type", nameof(Algorithm))]
    public async Task Algorithm_Unsupported_Throws()
    {
        //Given
        var value = "unsupported";

        //When
        Action action = () => _ = new Algorithm(value);

        //Then
        action.Should().Throw<NotSupportedException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a non-ecdsa algorithm, " +
        "When WithCurve is called, " +
        "Then it throws InvalidOperationException")]
    [Trait("Type", nameof(Algorithm))]
    public async Task WithCurve_NonEcdsa_Throws()
    {
        //Given
        var algorithm = new Algorithm(DigitalSignaturesAlgorithm.RsaSha256);

        //When
        Action action = () => algorithm.WithCurve(JsonWebKeyECTypes.P256);

        //Then
        action.Should().Throw<InvalidOperationException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an ecdsa algorithm, " +
        "When WithCurve is called, " +
        "Then it sets curve")]
    [Trait("Type", nameof(Algorithm))]
    public async Task WithCurve_Ecdsa_SetsCurve()
    {
        //Given
        var algorithm = new Algorithm(DigitalSignaturesAlgorithm.EcdsaSha256);

        //When
        algorithm.WithCurve(JsonWebKeyECTypes.P256);

        //Then
        algorithm.Curve.Should().Be(JsonWebKeyECTypes.P256);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a digital signature algorithm, " +
        "When WithContentEncryption is called, " +
        "Then it throws InvalidOperationException")]
    [Trait("Type", nameof(Algorithm))]
    public async Task WithContentEncryption_DigitalSignature_Throws()
    {
        //Given
        var algorithm = new Algorithm(DigitalSignaturesAlgorithm.RsaSha256);

        //When
        Action action = () => algorithm.WithContentEncryption(EncryptionAlgorithmContent.Aes128Gcm);

        //Then
        action.Should().Throw<InvalidOperationException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a JWE algorithm, " +
        "When WithContentEncryption is called, " +
        "Then it sets enc")]
    [Trait("Type", nameof(Algorithm))]
    public async Task WithContentEncryption_Jwe_SetsEnc()
    {
        //Given
        var algorithm = new Algorithm(EncryptionAlgorithmKey.RsaOAEP);

        //When
        algorithm.WithContentEncryption(EncryptionAlgorithmContent.Aes128Gcm);

        //Then
        algorithm.EncryptionAlgorithmContent.Enc.Should().Be(EncryptionAlgorithmContent.Aes128Gcm);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a JWE algorithm and invalid enc, " +
        "When WithContentEncryption is called, " +
        "Then it throws NotSupportedException")]
    [Trait("Type", nameof(Algorithm))]
    public async Task WithContentEncryption_Invalid_Throws()
    {
        //Given
        var algorithm = new Algorithm(EncryptionAlgorithmKey.RsaOAEP);
        var invalid = new EncryptionAlgorithmContent("INVALID");

        //When
        Action action = () => algorithm.WithContentEncryption(invalid);

        //Then
        action.Should().Throw<NotSupportedException>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given algorithm types, " +
        "When Kty is called, " +
        "Then it maps to key type")]
    [Trait("Type", nameof(Algorithm))]
    public async Task Kty_MapsKeyType()
    {
        //Given
        var rsa = new Algorithm(DigitalSignaturesAlgorithm.RsaSha256);
        var ecdsa = new Algorithm(DigitalSignaturesAlgorithm.EcdsaSha256);
        var hmac = new Algorithm(DigitalSignaturesAlgorithm.HmacSha256);
        var aes = new Algorithm(EncryptionAlgorithmKey.Aes128KW);

        //When
        var rsaKty = rsa.Kty();
        var ecdsaKty = ecdsa.Kty();
        var hmacKty = hmac.Kty();
        var aesKty = aes.Kty();

        //Then
        rsaKty.Should().Be(JsonWebAlgorithmsKeyTypes.RSA);
        ecdsaKty.Should().Be(JsonWebAlgorithmsKeyTypes.EllipticCurve);
        hmacKty.Should().Be(JsonWebAlgorithmsKeyTypes.Octet);
        aesKty.Should().Be(JsonWebAlgorithmsKeyTypes.Octet);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given Algorithm.Create with JWS, " +
        "When it is called, " +
        "Then it returns a JWS algorithm")]
    [Trait("Type", nameof(Algorithm))]
    public async Task Create_Jws_ReturnsJwsAlgorithm()
    {
        //Given
        //When
        var algorithm = Algorithm.Create(AlgorithmType.ECDsa, JwtType.Jws);

        //Then
        algorithm.JwtType.Should().Be(JwtType.Jws);
        algorithm.CryptographyType.Should().Be(CryptographyType.DigitalSignature);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given Algorithm.Create with JWE, " +
        "When it is called, " +
        "Then it returns a JWE algorithm")]
    [Trait("Type", nameof(Algorithm))]
    public async Task Create_Jwe_ReturnsJweAlgorithm()
    {
        //Given
        //When
        var algorithm = Algorithm.Create(AlgorithmType.RSA, JwtType.Jwe);

        //Then
        algorithm.JwtType.Should().Be(JwtType.Jwe);
        algorithm.CryptographyType.Should().Be(CryptographyType.Encryption);
        await Task.CompletedTask;
    }
}
