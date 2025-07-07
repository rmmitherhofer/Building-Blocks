using Common.Extensions;
using Common.Http.Configurations;
using Common.Notifications.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnapTrace.Applications;
using SnapTrace.BackgroundServices;
using SnapTrace.Builders;
using SnapTrace.Configurations.Settings;
using SnapTrace.Enums;
using SnapTrace.Extensions;
using SnapTrace.HttpServices;
using SnapTrace.Middleware;
using SnapTrace.Options;
using SnapTrace.Providers;
using SnapTrace.Queues;

namespace SnapTrace.Configurations;

/// <summary>
/// Extension methods for registering and configuring SnapTrace services and middleware.
/// </summary>
public static class SnapTraceConfigurations
{
    private const string SNAPTRACE_NODE = "SnapTraceSettings";

    /// <summary>
    /// Adds SnapTrace services to the specified <see cref="IServiceCollection"/> and configures it.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">Application configuration to bind SnapTrace settings.</param>
    /// <param name="configure">Action to configure <see cref="SnapTraceOptions"/>.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddSnapTrace(this IServiceCollection services, IConfiguration configuration, Action<SnapTraceOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        services.AddNotificationConfig();

        services.AddHttpConfig();

        services.AddOptions(configuration);

        services.AddAppService(configuration, configure);

        return services;
    }

    /// <summary>
    /// Registers SnapTrace configuration options from the provided configuration.
    /// </summary>
    /// <param name="services">The service collection to add options to.</param>
    /// <param name="configuration">The application configuration instance.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        services.Configure<SnapTraceSettings>(configuration.GetSection(SNAPTRACE_NODE));

        services.Configure<SensitiveDataMaskerOptions>(configuration.GetSection($"{SNAPTRACE_NODE}:SensitiveDataMasker"));

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SensitiveDataMaskerOptions>>().Value;

            var defaultKeys = new List<string>
            {
                "password", "senha",
                "token", "access_token", "refresh_token", "jwt", "jwe", "jws", "jwk", "jwa", "jwm",
                "auth", "authentication", "authorization", "autenticacao", "autorizacao",
                "secret", "client_secret", "api_key", "secret_key", "private_key", "assinatura", "signature", "segredo",
                "pin", "otp", "mfa_code", "codigo_mfa"
            };

            var mergedKeys = defaultKeys
                .Concat(options.SensitiveKeys ?? Enumerable.Empty<string>())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var finalOptions = new SensitiveDataMaskerOptions
            {
                SensitiveKeys = mergedKeys
            };

            return new SensitiveDataMasker(finalOptions);
        });

        return services;
    }

    /// <summary>
    /// Registers SnapTrace application services and logger provider.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configure">Action to configure <see cref="SnapTraceOptions"/>.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddAppService(this IServiceCollection services, IConfiguration configuration, Action<SnapTraceOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        SnapTraceOptions options = new();

        configure?.Invoke(options);

        var provider = services.BuildServiceProvider();
        var settings = provider.GetRequiredService<IOptions<SnapTraceSettings>>().Value;

        if (settings.ExecutionMode == SnapTraceExecutionMode.Disabled) return services;

        if (settings.ExecutionMode == SnapTraceExecutionMode.Full)
        {
            services.AddSingleton<ILoggerProvider, SnapTraceLoggerProvider>(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                return new SnapTraceLoggerProvider(options, httpContextAccessor);
            });
        }

        services.TryAddScoped<ISnapTraceApplication, SnapTraceApplication>();
        services.TryAddScoped<ILogContextBuilder, LogContextBuilder>();

        services.AddSingleton<ISnapTraceQueue, SnapTraceQueue>();
        services.AddHostedService<SnapTraceBackgroundService>();

        services.AddHttpService(configuration);

        return services;
    }

    /// <summary>
    /// Registers HTTP client services for SnapTrace integration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">Application configuration instance.</param>
    /// <returns>The updated service collection.</returns>
    private static IServiceCollection AddHttpService(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        services.AddHttpClient<ISnapTraceHttpService, SnapTraceHttpService>(client =>
            client.BaseAddress = new Uri(configuration.Get("Service:BaseAddress", SNAPTRACE_NODE))
        );

        return services;
    }

    /// <summary>
    /// Adds SnapTrace middleware components into the application's request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseSnapTrace(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        app.UseHttpConfig();

        var settings = app.ApplicationServices.GetRequiredService<IOptions<SnapTraceSettings>>().Value;

        if (settings.ExecutionMode == SnapTraceExecutionMode.Disabled) return app;

        app.TryUseMiddleware<SnapTraceMiddleware>();

        app.TryUseMiddleware<BodyBufferingMiddleware>();

        app.TryUseMiddleware<CaptureResponseBodyMiddleware>();

        return app;
    }
}
