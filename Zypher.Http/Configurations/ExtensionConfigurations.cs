﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Zypher.Http.Extensions;

namespace Zypher.Http.Configurations;

public static class ExtensionConfigurations
{
    /// <summary>
    /// Adds HTTP related services, including HttpContextAccessor, to the dependency injection container.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddHttpConfig(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        services.AddHttpContextAccessor();

        return services;
    }

    /// <summary>
    /// Configures the application to use HTTP related services by setting up IHttpContextAccessor
    /// for HttpClient and HttpRequestMessage extension classes.
    /// </summary>
    /// <param name="app">The IApplicationBuilder to configure.</param>
    /// <returns>The updated IApplicationBuilder.</returns>
    public static IApplicationBuilder UseHttpConfig(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        var httpContextAccessor = app.ApplicationServices.GetService<IHttpContextAccessor>()
            ?? throw new InvalidOperationException("IHttpContextAccessor is not registered. Make sure to call AddHttpConfig.");

        HttpClientExtensions.Configure(httpContextAccessor);

        HttpRequestMessageExtensions.Configure(httpContextAccessor);

        return app;
    }
}
