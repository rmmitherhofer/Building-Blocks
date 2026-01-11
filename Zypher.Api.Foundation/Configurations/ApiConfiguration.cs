using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NedMonitor.Configurations;
using NedMonitor.Core.Formatters;
using SwaggleBox.Configurations;
using System.Text.Json.Serialization;
using Zypher.Api.Foundation.Middleware;
using Zypher.Extensions.Core;
using Zypher.Logs.Configurations;
using Zypher.Notifications.Configurations;

namespace Zypher.Api.Foundation.Configurations;

/// <summary>
/// Provides configuration extensions for setting up core API services and middleware.
/// </summary>
public static class ApiConfiguration
{
    /// <summary>
    /// Registers core services for the API, such as controllers, Swagger, Tracezilla, and custom configurations.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="environment">The hosting environment.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddZypherApiFoundation(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        services.AddHttpContextAccessor();

        configuration.Set(environment);

        services.AddControllers(options => options.EnableEndpointRouting = false)
            .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

        services.AddNotificationConfig();

        services.AddEndpointsApiExplorer();

        services.AddSwaggleBox(configuration);

        services.AddNedMonitor(configuration, options =>
        {
            options.Formatter = (args) =>
            {
                if (args.Exception == null)
                    return args.DefaultValue;

                string exceptionStr = new ExceptionFormatter().Format(args.Exception);
                return string.Join(Environment.NewLine, [args.DefaultValue, exceptionStr]);
            };
        });

        return services;
    }

    /// <summary>
    /// Configures the HTTP request pipeline by registering routing, middleware, and endpoint mappings.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The updated application builder.</returns>
    public static IApplicationBuilder UseZypherApiFoundation(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        app.UseRouting();

        app.UseNedMonitor();

        app.TryUseMiddleware<RequestIndetityMiddleware>();

        app.UseNotificationConfig();

        app.UseLogDecoratorConfig();

        app.TryUseMiddleware<ExceptionMiddleware>();

        app.UseNedMonitorMiddleware();

        app.UseSwaggleBox(app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>());

        app.UseEndpoints(endpoints => endpoints.MapControllers());

        app.UseEndpoints(endpoints => endpoints.MapControllers());

        return app;
    }

    /// <summary>
    /// Configures the application to use authentication and authorization middleware.
    /// </summary>
    public static IApplicationBuilder UseUseAuthenticationFoundation(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
