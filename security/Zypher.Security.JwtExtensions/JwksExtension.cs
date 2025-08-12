using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Zypher.Security.JwtExtensions;

public static class JwksExtension
{
    public static void SetJwksOptions(this JwtBearerOptions options, JwkOptions jwkOptions)
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.IncludeErrorDetails = true;

        var httpClient = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler())
        {
            Timeout = options.BackchannelTimeout,
            MaxResponseContentBufferSize = 1024 * 1024 * 10 // 10 MB
        };

        options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            jwkOptions.JwksUri,
            new JwksRetriever(),
            new HttpDocumentRetriever(httpClient) { RequireHttps = options.RequireHttpsMetadata });

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireSignedTokens = true,
            ValidateLifetime = true,
        };

        if (!string.IsNullOrEmpty(jwkOptions.Issuer))
        {
            options.TokenValidationParameters.ValidateIssuer = true;
            options.TokenValidationParameters.ValidIssuer = jwkOptions.Issuer;
        }

        if (!string.IsNullOrEmpty(jwkOptions.Audience))
        {
            options.TokenValidationParameters.ValidateAudience = true;
            options.TokenValidationParameters.ValidAudience = jwkOptions.Audience;
        }
    }
}
