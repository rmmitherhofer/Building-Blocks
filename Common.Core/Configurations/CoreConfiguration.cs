using Common.Core.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Core.Configurations;

public static class CoreConfiguration
{
    public static IServiceCollection AddCoreConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        services.AddHttpContextAccessor();

        services.TryAddScoped<IAspNetUser, AspNetUser>();

        return services;
    }
}
