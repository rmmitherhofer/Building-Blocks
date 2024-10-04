using Api.Service.Middleware;
using Common.Notifications.Configurations;
using Extensoes;
using Logs.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swagger.Configurations;
using System.Text.Json.Serialization;

namespace Api.Service.Configurations;

public static class ApiConfiguration
{
    public static IServiceCollection AddCoreApiConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        configuration.SetConfiguration(environment);

        services.AddControllers(options => options.EnableEndpointRouting = false)
            .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

        services.AddNotificaticaoConfiguration();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerConfig();

        return services;
    }
    public static WebApplication UseCoreApiConfig(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(WebApplication));

        app.UseMiddleware<ConsoleLogMiddleware>();

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseSwaggerConfig(app.Services.GetRequiredService<IApiVersionDescriptionProvider>());

        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
