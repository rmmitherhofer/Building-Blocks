using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using Zypher.Security.Jwt.Core;
using Zypher.Security.Jwt.Core.Interfaces;
using Zypher.Security.Jwt.Core.Models;

namespace Zypher.Security.JwtSigningCredentials.AspNetCore;

public class JwtServiceDiscoveryMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    public async Task InvokeAsync(HttpContext context, IJwtService jwtService, IOptions<JwtOptions> options)
    {
        var storeKeys = await jwtService.GetLastKeys(options.Value.AlgorithmToKeep);

        var keys = new
        {
            keys = storeKeys.Select(x => x.GetSecurityKey()).Select(PublicJsonWebKey.FromJwk)
        };

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(keys, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }));
    }
}
