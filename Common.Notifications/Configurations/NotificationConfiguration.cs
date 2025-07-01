using Common.Extensions.Configurations;
using Common.Notifications.Handlers;
using Common.Notifications.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Notifications.Configurations;

public static class NotificationConfiguration
{
    /// <summary>
    /// Configura e resolve o contrado do INotificationHandler
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection</returns>
    public static IServiceCollection AddNotificationConfig(this IServiceCollection services)
    {
        services.AddExtensionConfig();

        services.TryAddScoped<INotificationHandler, NotificationHandler>();

        return services;
    }

    /// <summary>
    /// Configura e resolve o contrado do INotificationHandler
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <returns>IServiceCollection</returns>
    public static IApplicationBuilder UseNotificationConfig(this IApplicationBuilder app)
    {
        app.UseExtensionConfig();

        return app;
    }
}