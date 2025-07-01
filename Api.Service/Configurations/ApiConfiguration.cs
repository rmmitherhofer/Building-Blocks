using Api.Service.Middleware;
using Common.Logs.Configurations;
using Common.Notifications.Configurations;
using Extensoes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SnapTrace.Configurations;
using SnapTrace.Formatters;
using Swagger.Configurations;
using System.Text.Json.Serialization;

namespace Api.Service.Configurations;

public static class ApiConfiguration
{
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
    public static IApplicationBuilder UseCoreApiConfig(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        app.UseRouting();

        app.UseAuthorization();

        app.UseSnapTrace();

        app.TryUseMiddleware<RequestIndetityMiddleware>(RequestIndetityMiddleware.Name);

        app.UseNotificationConfig();

        app.UseLogDecoratorConfig();

        app.TryUseMiddleware<ExceptionMiddleware>(ExceptionMiddleware.Name);

        app.UseSwaggerConfig(app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>());

        app.UseEndpoints(endpoints => endpoints.MapControllers());

        return app;
    }
}
