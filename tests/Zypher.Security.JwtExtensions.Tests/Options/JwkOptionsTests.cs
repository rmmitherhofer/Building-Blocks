using System;
using FluentAssertions;
using Xunit;
using Zypher.Security.JwtExtensions;

namespace Zypher.Security.JwtExtensions.Tests.Options;

public class JwkOptionsTests
{
    [Fact(DisplayName =
        "Given a JWKS URI, " +
        "When JwkOptions is created, " +
        "Then it sets defaults from the URI")]
    [Trait("Type", nameof(JwkOptions))]
    public async Task JwkOptions_Ctor_SetsDefaults()
    {
        //Given
        var uri = "https://example.com/.well-known/jwks.json";

        //When
        var options = new JwkOptions(uri);

        //Then
        options.JwksUri.Should().Be(uri);
        options.Issuer.Should().Be("https://example.com");
        options.KeepFor.Should().Be(TimeSpan.FromMinutes(15));
        options.Audience.Should().BeNull();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given custom values, " +
        "When JwkOptions is created, " +
        "Then it uses provided values")]
    [Trait("Type", nameof(JwkOptions))]
    public async Task JwkOptions_Ctor_UsesCustomValues()
    {
        //Given
        var uri = "https://example.com/.well-known/jwks.json";
        var keep = TimeSpan.FromMinutes(5);

        //When
        var options = new JwkOptions(uri, issuer: "issuer", cacheTime: keep, audience: "aud");

        //Then
        options.Issuer.Should().Be("issuer");
        options.KeepFor.Should().Be(keep);
        options.Audience.Should().Be("aud");
        await Task.CompletedTask;
    }
}
