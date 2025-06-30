using Extensoes;
using Logs.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Common.Logs.Configurations;

public static class LogConfigurations
{
    public static IApplicationBuilder UseLogDecoratorConfig(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        app.TryUseMiddleware<LogDecoratorMiddleware>(LogDecoratorMiddleware.Name);

        return app;
    }
}

