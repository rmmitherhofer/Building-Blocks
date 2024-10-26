using Common.Http.Configurations;
using Common.Notifications.Configurations;
using Extensoes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SnapTrace.Applications;
using SnapTrace.Configurations.Settings;
using SnapTrace.HttpServices;
using SnapTrace.Middleware;

namespace SnapTrace.Configurations;

public static class SnapTraceConfigurations
{
    public static IServiceCollection AddSnapTrace(this IServiceCollection services, IConfiguration configuration, SnapTraceSettings settings)
    {
        services.AddNotificationConfig();

        services.TryAddSingleton(settings);

        services.AddHttpConfig();

        services.AddAppService();

        services.AddHttpService(configuration);

        return services;
    }

    public static IServiceCollection AddAppService(this IServiceCollection services)
    {
        services.TryAddScoped<ISnapTraceApplication, SnapTraceApplication>();

        return services;
    }

    public static IServiceCollection AddHttpService(this IServiceCollection services, IConfiguration configuration)
    {
        const string SECTION = "Apis";

        services.AddHttpClient<ILogHttpService, LogHttpService>(services =>
            services.BaseAddress = new Uri(configuration.Get("SnapTrace:BaseAddress", SECTION))
        );

        return services;
    }

    public static WebApplication UseSnapTrace(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(WebApplication));

        app.UseMiddleware<BodyBufferingMiddleware>();

        return app;
    }
}
