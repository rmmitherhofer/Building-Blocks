using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Zypher.Security.Jwt.Core.Interfaces;

namespace Zypher.Security.JwtSigningCredentials.AspNetCore;

public static class AspNetBuilderExtensions
{
    public static IApplicationBuilder UseJwksDiscovery(this IApplicationBuilder app, string jwtDiscoveryEndpoint = "/jwks")
    {
        if (!jwtDiscoveryEndpoint.StartsWith("/")) throw new ArgumentException("The jwks URI must start with a '/' character.", nameof(jwtDiscoveryEndpoint));

        app.Map(new PathString(jwtDiscoveryEndpoint), builder => builder.UseMiddleware<JwtServiceDiscoveryMiddleware>());

        return app;
    }

    public static IJwksBuilder UseJwtValidation(this IJwksBuilder builder)
    {
        builder.Services.TryAddSingleton<IPostConfigureOptions<JwtBearerOptions>>(x => new JwtPostConfigureOptions(x));

        return builder;
    }
}
