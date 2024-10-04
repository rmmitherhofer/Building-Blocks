using Common.Notifications.Handlers;
using Common.Notifications.Interfaces;
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
    public static IServiceCollection AddNotificaticaoConfiguration(this IServiceCollection services)
    {
        services.TryAddScoped<INotificationHandler, NotificationHandler>();

        return services;
    }
}