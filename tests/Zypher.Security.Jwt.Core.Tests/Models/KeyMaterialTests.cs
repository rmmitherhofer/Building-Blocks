using System;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Zypher.Security.Jwt.Core.Jwa;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Security.Jwt.Core.Tests.Models;

public class KeyMaterialTests
{
    [Fact(DisplayName =
        "Given a cryptographic key, " +
        "When KeyMaterial is created, " +
        "Then it sets fields")]
    [Trait("Type", nameof(KeyMaterial))]
    public async Task KeyMaterial_Ctor_SetsFields()
    {
        //Given
        var crypto = new CryptographicKey(new Algorithm(DigitalSignaturesAlgorithm.RsaSha256));

        //When
        var material = new KeyMaterial(crypto);

        //Then
        material.Id.Should().NotBeEmpty();
        material.KeyId.Should().Be(crypto.Key.KeyId);
        material.Type.Should().Be(crypto.Algorithm.Kty());
        material.Parameters.Should().NotBeNullOrEmpty();
        material.CreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given KeyMaterial, " +
        "When GetSecurityKey is called, " +
        "Then it returns JsonWebKey")]
    [Trait("Type", nameof(KeyMaterial))]
    public async Task KeyMaterial_GetSecurityKey_ReturnsJwk()
    {
        //Given
        var crypto = new CryptographicKey(new Algorithm(DigitalSignaturesAlgorithm.RsaSha256));
        var material = new KeyMaterial(crypto);

        //When
        var key = material.GetSecurityKey();

        //Then
        key.Should().BeOfType<JsonWebKey>();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given KeyMaterial, " +
        "When Revoke is called, " +
        "Then it marks it revoked")]
    [Trait("Type", nameof(KeyMaterial))]
    public async Task KeyMaterial_Revoke_MarksRevoked()
    {
        //Given
        var crypto = new CryptographicKey(new Algorithm(DigitalSignaturesAlgorithm.RsaSha256));
        var material = new KeyMaterial(crypto);

        //When
        material.Revoke("reason");

        //Then
        material.IsRevoked.Should().BeTrue();
        material.RevokedReason.Should().Be("reason");
        material.ExpiredAt.Should().NotBeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given an old KeyMaterial, " +
        "When IsExpired is called, " +
        "Then it returns true")]
    [Trait("Type", nameof(KeyMaterial))]
    public async Task KeyMaterial_IsExpired_ReturnsTrue()
    {
        //Given
        var crypto = new CryptographicKey(new Algorithm(DigitalSignaturesAlgorithm.RsaSha256));
        var material = new KeyMaterial(crypto)
        {
            CreationDate = DateTime.UtcNow.AddDays(-10)
        };

        //When
        var result = material.IsExpired(5);

        //Then
        result.Should().BeTrue();
        await Task.CompletedTask;
    }
}
