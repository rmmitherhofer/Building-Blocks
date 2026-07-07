using System;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Zypher.Security.JwtSigningCredentials;
using Zypher.Security.JwtSigningCredentials.Models;

namespace Zypher.Security.JwtSigningCredentials.Tests.Models;

public class SecurityKeyWithPrivateTests
{
    [Fact(DisplayName =
        "Given a security key and algorithm, " +
        "When SetParameters is called, " +
        "Then it sets fields")]
    [Trait("Type", nameof(SecurityKeyWithPrivate))]
    public async Task SetParameters_SetsFields()
    {
        //Given
        var symmetric = new SymmetricSecurityKey(new byte[32]) { KeyId = "kid" };
        var key = JsonWebKeyConverter.ConvertFromSymmetricSecurityKey(symmetric);
        var model = new SecurityKeyWithPrivate();

        //When
        model.SetParameters(key, Algorithm.HS256);

        //Then
        model.KeyId.Should().Be("kid");
        model.Type.Should().Be(Algorithm.HS256.Kty());
        model.Parameters.Should().NotBeNullOrEmpty();
        model.Algorithm.Should().Be(Algorithm.HS256);
        model.CreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given parameters, " +
        "When GetSecurityKey is called, " +
        "Then it returns JsonWebKey")]
    [Trait("Type", nameof(SecurityKeyWithPrivate))]
    public async Task GetSecurityKey_ReturnsJwk()
    {
        //Given
        var symmetric = new SymmetricSecurityKey(new byte[32]) { KeyId = "kid" };
        var key = JsonWebKeyConverter.ConvertFromSymmetricSecurityKey(symmetric);
        var model = new SecurityKeyWithPrivate();
        model.SetParameters(key, Algorithm.HS256);

        //When
        var jwk = model.GetSecurityKey();

        //Then
        jwk.Should().BeOfType<JsonWebKey>();
        await Task.CompletedTask;
    }
}
