using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Zypher.Security.JwtSigningCredentials.AspNetCore;

namespace Zypher.Security.JwtSigningCredentials.AspNetCore.Tests.Middleware;

public class JwtPostConfigureOptionsTests
{
    [Fact(DisplayName =
        "Given JwtBearerOptions, " +
        "When PostConfigure is called, " +
        "Then it replaces token handlers")]
    [Trait("Type", nameof(JwtPostConfigureOptions))]
    public async Task PostConfigure_ReplacesTokenHandlers()
    {
        //Given
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var options = new JwtBearerOptions();
        options.TokenHandlers.Add(new JwtSecurityTokenHandler());

        //When
        var post = new JwtPostConfigureOptions(provider);
        post.PostConfigure(null, options);

        //Then
        options.TokenHandlers.Should().ContainSingle()
            .Which.Should().BeOfType<JwtServiceValidationHandler>();
        await Task.CompletedTask;
    }
}
