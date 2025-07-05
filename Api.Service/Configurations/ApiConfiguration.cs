using Api.Service.Middleware;
using Api.Swagger.Configurations;
using Common.Extensions;
using Common.Logs.Configurations;
using Common.Notifications.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SnapTrace.Configurations;
using SnapTrace.Formatters;
using System.Text.Json.Serialization;

namespace Api.Service.Configurations;

/// <summary>
/// Provides configuration extensions for setting up core API services and middleware.
/// </summary>
public static class ApiConfiguration
{
    /// <summary>
    /// Registers core services for the API, such as controllers, Swagger, SnapTrace, and custom configurations.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="environment">The hosting environment.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCoreApiConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        services.AddHttpContextAccessor();

        configuration.Set(environment);

        services.AddControllers(options => options.EnableEndpointRouting = false)
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        services.AddNotificationConfig();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerConfig(configuration);

        services.AddSnapTrace(configuration, options =>
        {
            options.Formatter = (FormatterArgs args) =>
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
    public static IApplicationBuilder UseCoreApiConfig(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        app.UseRouting();

        app.UseAuthorization();

        app.UseSnapTrace();

        app.TryUseMiddleware<RequestIndetityMiddleware>();

        app.UseNotificationConfig();

        app.UseLogDecoratorConfig();

        app.TryUseMiddleware<ExceptionMiddleware>();

        app.UseSwaggerConfig(app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>());

        app.UseEndpoints(endpoints => endpoints.MapControllers());

        return app;
    }
}
