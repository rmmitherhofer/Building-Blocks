using System;
using FluentAssertions;
using Xunit;
using Zypher.Security.JwtSigningCredentials;
using Zypher.Security.JwtSigningCredentials.Jwk;

namespace Zypher.Security.JwtSigningCredentials.Tests.Jwk;

public class AlgorithmTests
{
    [Fact(DisplayName =
        "Given RSA algorithm, " +
        "When Kty is called, " +
        "Then it returns RSA")]
    [Trait("Type", nameof(Algorithm))]
    public async Task Kty_Rsa_ReturnsRsa()
    {
        //Given
        var algorithm = Algorithm.RS256;

        //When
        var kty = algorithm.Kty();

        //Then
        kty.Should().Be(Microsoft.IdentityModel.Tokens.JsonWebAlgorithmsKeyTypes.RSA);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given ECDsa algorithm, " +
        "When Kty is called, " +
        "Then it returns EllipticCurve")]
    [Trait("Type", nameof(Algorithm))]
    public async Task Kty_Ecdsa_ReturnsEllipticCurve()
    {
        //Given
        var algorithm = Algorithm.ES256;

        //When
        var kty = algorithm.Kty();

        //Then
        kty.Should().Be(Microsoft.IdentityModel.Tokens.JsonWebAlgorithmsKeyTypes.EllipticCurve);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given HMAC algorithm, " +
        "When Kty is called, " +
        "Then it returns Octet")]
    [Trait("Type", nameof(Algorithm))]
    public async Task Kty_Hmac_ReturnsOctet()
    {
        //Given
        var algorithm = Algorithm.HS256;

        //When
        var kty = algorithm.Kty();

        //Then
        kty.Should().Be(Microsoft.IdentityModel.Tokens.JsonWebAlgorithmsKeyTypes.Octet);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an invalid key type, " +
        "When Algorithm is created, " +
        "Then Kty throws")]
    [Trait("Type", nameof(Algorithm))]
    public async Task Kty_InvalidKeyType_Throws()
    {
        //Given
        var algorithm = new Algorithm("custom", (KeyType)99);

        //When
        Action action = () => algorithm.Kty();

        //Then
        action.Should().Throw<ArgumentOutOfRangeException>();
        await Task.CompletedTask;
    }
}
