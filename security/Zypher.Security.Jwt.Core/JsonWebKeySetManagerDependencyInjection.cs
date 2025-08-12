using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zypher.Security.Jwt.Core.DefaultStore;
using Zypher.Security.Jwt.Core.Interfaces;
using Zypher.Security.Jwt.Core.Jwt;

namespace Zypher.Security.Jwt.Core;

public static class JsonWebKeySetManagerDependencyInjection
{
    public static IJwksBuilder AddJwksManager(this IServiceCollection services, Action<JwtOptions>? action = null)
    {
        if (action is not null) services.Configure(action);

        services.AddDataProtection();
        services.TryAddScoped<IJwtService, JwtService>();
        services.AddScoped<IJsonWebKeyStore, DataProtectionStore>();

        return new JwksBuilder(services);
    }

    public static IJwksBuilder PrsistKeysInMemory(this IJwksBuilder builder)
    {
        builder.Services.AddScoped<IJsonWebKeyStore, InMemoryStore>();

        return builder;
    }
}
