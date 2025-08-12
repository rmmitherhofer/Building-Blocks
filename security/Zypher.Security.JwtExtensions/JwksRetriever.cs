using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Zypher.Security.JwtExtensions;

public class JwksRetriever : IConfigurationRetriever<OpenIdConnectConfiguration>
{
    public Task<OpenIdConnectConfiguration> GetConfigurationAsync(string address, IDocumentRetriever retriever, CancellationToken cancel) 
        => GetAsync(address, retriever, cancel);

    public static async Task<OpenIdConnectConfiguration> GetAsync(string address, IDocumentRetriever retriever, CancellationToken cancel)
    {
        if (string.IsNullOrEmpty(address))
            throw LogHelper.LogArgumentNullException(nameof(address));


        if (retriever is null)
            throw LogHelper.LogArgumentNullException(nameof(retriever));

        IdentityModelEventSource.ShowPII = true;

        var document = await retriever.GetDocumentAsync(address, cancel);

        LogHelper.LogInformation("IDX21811: Deserializing the string '{0}' obtained from metadata endpoint into openIdConnectConfiguration object", document);

        var jwks = new JsonWebKeySet(document);

        OpenIdConnectConfiguration openIdConnectConfiguration = new()
        {
            JsonWebKeySet = jwks,
            JwksUri = address,
        };

        foreach (var securityKey in jwks.GetSigningKeys())
            openIdConnectConfiguration.SigningKeys.Add(securityKey);

        return openIdConnectConfiguration;
    }
}