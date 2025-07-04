using Common.Notifications.Configurations;
using Extensoes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnapTrace.Adapters;
using SnapTrace.Applications;
using SnapTrace.Builders;
using SnapTrace.Configurations.Settings;
using SnapTrace.Extensions;
using SnapTrace.HttpServices;
using SnapTrace.Middleware;

namespace SnapTrace.Configurations;

public static class SnapTraceConfigurations
{
    const string SNAPTRACE_NODE = "SnapTraceSettings";
    public static IServiceCollection AddSnapTrace(this IServiceCollection services, IConfiguration configuration, Action<LoggerOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        services.AddNotificationConfig();

        services.AddOptions(configuration);

        services.AddAppService(configure);

        services.AddHttpService(configuration);

        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        services.Configure<SnapTraceSettings>(configuration.GetSection(SNAPTRACE_NODE));

        services.Configure<SensitiveDataMaskerOptions>(configuration.GetSection($"{SNAPTRACE_NODE}:SensitiveDataMasker"));

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SensitiveDataMaskerOptions>>().Value;
            return new SensitiveDataMasker(options);
        });

        return services;
    }

    private static IServiceCollection AddAppService(this IServiceCollection services, Action<LoggerOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        LoggerOptions options = new();

        configure?.Invoke(options);

        services.AddSingleton<ILoggerProvider, SnapTraceLoggerProvider>(sp =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            return new SnapTraceLoggerProvider(options, httpContextAccessor);
        });

        services.TryAddScoped<ISnapTraceApplication, SnapTraceApplication>();
        services.TryAddScoped<ILogContextBuilder, LogContextBuilder>();

        return services;
    }

    private static IServiceCollection AddHttpService(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        services.AddHttpClient<ISnapTraceHttpService, SnapTraceHttpService>(services =>
            services.BaseAddress = new Uri(configuration.Get("Service:BaseAddress", SNAPTRACE_NODE))
        );

        return services;
    }

    public static IApplicationBuilder UseSnapTrace(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        app.TryUseMiddleware<SnapTraceMiddleware>(SnapTraceMiddleware.Name);

        app.TryUseMiddleware<BodyBufferingMiddleware>(BodyBufferingMiddleware.Name);

        app.TryUseMiddleware<CaptureResponseBodyMiddleware>(CaptureResponseBodyMiddleware.Name);

        return app;
    }
}
