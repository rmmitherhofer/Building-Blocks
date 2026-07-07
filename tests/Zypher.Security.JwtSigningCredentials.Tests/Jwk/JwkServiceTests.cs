using System;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Zypher.Security.JwtSigningCredentials;
using Zypher.Security.JwtSigningCredentials.Jwk;

namespace Zypher.Security.JwtSigningCredentials.Tests.Jwk;

public class JwkServiceTests
{
    [Fact(DisplayName =
        "Given an RSA algorithm, " +
        "When Generate is called, " +
        "Then it returns a JsonWebKey")]
    [Trait("Type", nameof(JwkService))]
    public async Task Generate_Rsa_ReturnsJwk()
    {
        //Given
        var service = new JwkService();

        //When
        var jwk = service.Generate(Algorithm.RS256);

        //Then
        jwk.Should().NotBeNull();
        jwk.Kty.Should().Be(JsonWebAlgorithmsKeyTypes.RSA);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a null key, " +
        "When GenereteSigningCredentials is called, " +
        "Then it throws ArgumentNullException")]
    [Trait("Type", nameof(JwkService))]
    public async Task GenereteSigningCredentials_NullKey_Throws()
    {
        //Given
        var service = new JwkService();

        //When
        Action action = () => service.GenereteSigningCredentials(null!, Algorithm.RS256);

        //Then
        action.Should().Throw<ArgumentNullException>();
        await Task.CompletedTask;
    }
}
