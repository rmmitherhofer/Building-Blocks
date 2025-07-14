using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Zypher.Extensions.Core;
using Zypher.Logs.Extensions;
using Zypher.Logs.Middlewares;

namespace Zypher.Logs.Configurations;

/// <summary>
/// Extension methods for configuring logging services and middleware.
/// </summary>
public static class LogConfigurations
{
    /// <summary>
    /// Adds HttpContextAccessor service needed for logging extensions.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddConsoleLogExtensionConfig(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        services.AddHttpContextAccessor();

        return services;
    }

    /// <summary>
    /// Adds the LogDecoratorMiddleware to the application's middleware pipeline.
    /// </summary>
    /// <param name="app">The IApplicationBuilder instance.</param>
    /// <returns>The updated IApplicationBuilder.</returns>
    public static IApplicationBuilder UseLogDecoratorConfig(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        app.TryUseMiddleware<LogDecoratorMiddleware>();

        return app;
    }

    /// <summary>
    /// Configures the ConsoleLogExtensions with the registered IHttpContextAccessor.
    /// </summary>
    /// <param name="app">The IApplicationBuilder instance.</param>
    /// <returns>The updated IApplicationBuilder.</returns>
    public static IApplicationBuilder UseConsoleLogExtensionConfig(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        var httpContextAccessor = app.ApplicationServices.GetService<IHttpContextAccessor>()
            ?? throw new InvalidOperationException("IHttpContextAccessor is not registered. Make sure to call AddConsoleLogExtensionConfig.");

        ConsoleLogExtensions.Configure(httpContextAccessor);

        return app;
    }
}
