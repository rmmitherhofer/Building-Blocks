using System;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Xunit;
using Zypher.Security.JwtExtensions;

namespace Zypher.Security.JwtExtensions.Tests.Extensions;

public class JwksExtensionTests
{
    [Fact(DisplayName =
        "Given JWT options and JWKS options, " +
        "When SetJwksOptions is called, " +
        "Then it configures validation parameters")]
    [Trait("Type", nameof(JwksExtension))]
    public async Task SetJwksOptions_ConfiguresParameters()
    {
        //Given
        var jwtOptions = new JwtBearerOptions
        {
            BackchannelTimeout = TimeSpan.FromSeconds(5)
        };
        var jwkOptions = new JwkOptions("https://example.com/jwks.json", issuer: "issuer", audience: "aud");

        //When
        jwtOptions.SetJwksOptions(jwkOptions);

        //Then
        jwtOptions.RequireHttpsMetadata.Should().BeTrue();
        jwtOptions.SaveToken.Should().BeTrue();
        jwtOptions.IncludeErrorDetails.Should().BeTrue();
        jwtOptions.TokenValidationParameters.ValidateIssuer.Should().BeTrue();
        jwtOptions.TokenValidationParameters.ValidIssuer.Should().Be("issuer");
        jwtOptions.TokenValidationParameters.ValidateAudience.Should().BeTrue();
        jwtOptions.TokenValidationParameters.ValidAudience.Should().Be("aud");
        jwtOptions.TokenValidationParameters.RequireSignedTokens.Should().BeTrue();
        jwtOptions.TokenValidationParameters.ValidateLifetime.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Fact(DisplayName =
        "Given missing issuer and audience, " +
        "When SetJwksOptions is called, " +
        "Then it keeps validation disabled")]
    [Trait("Type", nameof(JwksExtension))]
    public async Task SetJwksOptions_NoIssuerAudience_DisablesValidation()
    {
        //Given
        var jwtOptions = new JwtBearerOptions();
        var jwkOptions = new JwkOptions("https://example.com/jwks.json")
        {
            Issuer = null!,
            Audience = null
        };

        //When
        jwtOptions.SetJwksOptions(jwkOptions);

        //Then
        jwtOptions.TokenValidationParameters.ValidateIssuer.Should().BeFalse();
        jwtOptions.TokenValidationParameters.ValidateAudience.Should().BeFalse();
        await Task.CompletedTask;
    }
}
