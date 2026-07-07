using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Zypher.Http.Extensions;
using Zypher.Logs.Configurations;
using Zypher.Notifications.Configurations;

namespace Zypher.Http.Configurations;

public static class ExtensionConfigurations
{
    /// <summary>
    /// Adds HTTP related services, including HttpContextAccessor, to the dependency injection container.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddZypherHttp(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        services.AddHttpContextAccessor();
        services.AddZypherNotification();
        services.AddZypherLog();

        return services;
    }

    /// <summary>
    /// Configures the application to use HTTP related services by setting up IHttpContextAccessor
    /// for HttpClient and HttpRequestMessage extension classes.
    /// </summary>
    /// <param name="app">The IApplicationBuilder to configure.</param>
    /// <returns>The updated IApplicationBuilder.</returns>
    public static IApplicationBuilder UseZypherHttp(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        app.UseZypherLog();

        var httpContextAccessor = app.ApplicationServices.GetService<IHttpContextAccessor>()
            ?? throw new InvalidOperationException("IHttpContextAccessor is not registered. Make sure to call AddHttpConfig.");

        HttpRequestMessageExtensions.Configure(httpContextAccessor);

        return app;
    }
}
