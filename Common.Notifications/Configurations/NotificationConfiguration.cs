using Common.Logs.Configurations;
using Common.Notifications.Handlers;
using Common.Notifications.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Notifications.Configurations;

/// <summary>
/// Provides extension methods to configure notification services and middleware.
/// </summary>
public static class NotificationConfiguration
{
    /// <summary>
    /// Adds notification services and related logging configuration to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddNotificationConfig(this IServiceCollection services)
    {
        services.AddConsoleLogExtensionConfig();

        services.TryAddScoped<INotificationHandler, NotificationHandler>();

        return services;
    }

    /// <summary>
    /// Adds notification related middleware to the application builder pipeline.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> to configure.</param>
    /// <returns>The updated <see cref="IApplicationBuilder"/>.</returns>
    public static IApplicationBuilder UseNotificationConfig(this IApplicationBuilder app)
    {
        app.UseConsoleLogExtensionConfig();

        return app;
    }
}
