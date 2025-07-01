using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extensions.Configurations;

public static class ExtensionConfigurations
{
    public static IServiceCollection AddExtensionConfig(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        services.AddHttpContextAccessor();

        return services;
    }

    public static IApplicationBuilder UseExtensionConfig(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        ConsoleLogExtensions.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

        return app;
    }
}

