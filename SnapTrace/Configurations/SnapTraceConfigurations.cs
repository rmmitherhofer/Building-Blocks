using Common.Http.Configurations;
using Common.Notifications.Configurations;
using Extensoes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SnapTrace.Adapters;
using SnapTrace.Applications;
using SnapTrace.Configurations.Settings;
using SnapTrace.HttpServices;
using SnapTrace.Middleware;

namespace SnapTrace.Configurations;

public static class SnapTraceConfigurations
{
    public static IServiceCollection AddSnapTrace(this IServiceCollection services, IConfiguration configuration, SnapTraceSettings settings, Action<LoggerOptions> configure)
    {
        services.AddNotificationConfig();

        services.TryAddSingleton(settings);

        services.AddHttpContextAccessor();

        services.AddHttpConfig();

        services.AddAppService(configure);

        services.AddHttpService(configuration);

        return services;
    }

    private static IServiceCollection AddAppService(this IServiceCollection services, Action<LoggerOptions> configure)
    {
        LoggerOptions options = new();

        configure?.Invoke(options);

        services.AddSingleton<ILoggerProvider, SnapTraceLoggerProvider>(sp =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            return new SnapTraceLoggerProvider(options, httpContextAccessor);
        });

        services.AddScoped<ISnapTraceApplication, SnapTraceApplication>();

        return services;
    }

    private static IServiceCollection AddHttpService(this IServiceCollection services, IConfiguration configuration)
    {
        const string SECTION = "Apis";

        services.AddHttpClient<ILogHttpService, LogHttpService>(services =>
            services.BaseAddress = new Uri(configuration.Get("SnapTrace:BaseAddress", SECTION))
        );

        return services;
    }

    public static IApplicationBuilder UseSnapTrace(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        app.UseMiddleware<BodyBufferingMiddleware>();

        app.UseMiddleware<SnapTraceMiddleware>();

        return app;
    }
}
