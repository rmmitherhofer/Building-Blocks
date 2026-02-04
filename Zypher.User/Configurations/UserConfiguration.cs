using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zypher.User.Users;

namespace Zypher.User.Configurations;

public static class UserConfiguration
{
    /// <summary>
    /// Adds core configuration services to the dependency injection container,
    /// including HttpContextAccessor and AspNetUser service.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddAspNetUser(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        services.AddHttpContextAccessor();

        services.TryAddScoped<IAspNetUser, AspNetUser>();

        return services;
    }
}
