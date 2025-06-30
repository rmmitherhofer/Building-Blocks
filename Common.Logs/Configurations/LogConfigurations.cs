using Logs.Extensions;
using Logs.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Logs.Configurations;

public static class LogConfigurations
{
    public static IServiceCollection AddLogConfig(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));

        services.AddHttpContextAccessor();

        return services;
    }

    public static IApplicationBuilder UseLogConfig(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        ConsoleLog.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

        app.UseMiddleware<LogMiddleware>();

        return app;
    }
}

