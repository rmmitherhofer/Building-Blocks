using Microsoft.Extensions.DependencyInjection;

namespace Common.Http.Configurations;

public static class HttpConfiguration
{
    public static IServiceCollection AddHttpConfig(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        return services;
    }
}
