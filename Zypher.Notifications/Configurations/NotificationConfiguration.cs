using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zypher.Notifications.Handlers;
using Zypher.Notifications.Interfaces;

namespace Zypher.Notifications.Configurations;

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
    public static IServiceCollection AddZypherNotification(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        services.AddHttpContextAccessor();
        services.TryAddScoped<INotificationHandler, NotificationHandler>();

        return services;
    }
}
