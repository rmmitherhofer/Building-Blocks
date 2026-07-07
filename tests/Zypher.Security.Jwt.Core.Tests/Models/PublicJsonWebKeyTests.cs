using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using Zypher.Security.Jwt.Core.Jwa;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Security.Jwt.Core.Tests.Models;

public class PublicJsonWebKeyTests
{
    [Fact(DisplayName =
        "Given a JsonWebKey, " +
        "When PublicJsonWebKey is created, " +
        "Then it maps key fields")]
    [Trait("Type", nameof(PublicJsonWebKey))]
    public async Task PublicJsonWebKey_FromJwk_MapsFields()
    {
        //Given
        var crypto = new CryptographicKey(new Algorithm(DigitalSignaturesAlgorithm.RsaSha256));
        var jwk = crypto.GetJsonWebKey();
        jwk.Alg = DigitalSignaturesAlgorithm.RsaSha256;

        //When
        var publicKey = new PublicJsonWebKey(jwk);

        //Then
        publicKey.KeyType.Should().Be(jwk.Kty);
        publicKey.KeyId.Should().Be(jwk.Kid);
        publicKey.Algorithm.Should().Be(jwk.Alg);
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given a PublicJsonWebKey, " +
        "When ToNativeJwk is called, " +
        "Then it recreates a JsonWebKey")]
    [Trait("Type", nameof(PublicJsonWebKey))]
    public async Task PublicJsonWebKey_ToNativeJwk_RecreatesJwk()
    {
        //Given
        var crypto = new CryptographicKey(new Algorithm(DigitalSignaturesAlgorithm.RsaSha256));
        var jwk = crypto.GetJsonWebKey();
        jwk.Alg = DigitalSignaturesAlgorithm.RsaSha256;
        var publicKey = new PublicJsonWebKey(jwk);

        //When
        var native = publicKey.ToNativeJwk();

        //Then
        native.Kty.Should().Be(jwk.Kty);
        native.Kid.Should().Be(jwk.Kid);
        native.Alg.Should().Be(jwk.Alg);
        await Task.CompletedTask;
    }
}
