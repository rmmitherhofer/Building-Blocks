using Microsoft.AspNetCore.Builder;

namespace Extensoes;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder TryUseMiddleware<TMiddleware>(this IApplicationBuilder app, string key)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(IApplicationBuilder));

        if (!app.Properties.ContainsKey(key))
        {
            app.UseMiddleware<TMiddleware>();
            app.Properties[key] = true;
        }
        return app;
    }
}