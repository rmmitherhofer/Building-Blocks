using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zypher.Security.JwtSigningCredentials.Interfaces;
using Zypher.Security.JwtSigningCredentials.Jwks;

namespace Zypher.Security.JwtSigningCredentials;

public static class JsonWebKeySetManagerDependencyInjection
{
    public static IJwksBuilder AddJwksManager(this IServiceCollection services, Action<JwksOptions> action = null)
    {
        if (action is not null)
            services.Configure(action);

        services.TryAddScoped<IJsonWebKeySetService, JwksService>();
        services.TryAddScoped<IJsonWebKeySetService, JwksService>();
        services.TryAddSingleton<IJsonWebKeyStore, InMemoryStore>();

        return new JwksBuilder(services);
    }

    public static IJwksBuilder PersistKeysMemory(this IJwksBuilder builder)
    {
        builder.Services.AddSingleton<IJsonWebKeyStore, InMemoryStore>();

        return builder;
    }
}
