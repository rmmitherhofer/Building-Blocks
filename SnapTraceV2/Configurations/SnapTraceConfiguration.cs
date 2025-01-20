using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using SnapTraceV2.Builders;
using SnapTraceV2.Middlewares;
using SnapTraceV2.Options;
using SnapTraceV2.Providers;

namespace SnapTraceV2.Configurations;

public static class SnapTraceConfiguration
{
    public static IApplicationBuilder UseSnapTraceMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseSnapTraceMiddleware(null);
    }

    public static IApplicationBuilder UseSnapTraceMiddleware(this IApplicationBuilder builder, Action<IOptionsBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(IApplicationBuilder));

        configure?.Invoke(new OptionsBuilder());

        return builder.UseMiddleware<SnapTraceMiddleware>();
    }

    public static ILoggingBuilder AddSnapTrace(this ILoggingBuilder builder)
    {
        return builder.AddSnapTrace(null);
    }

    public static ILoggingBuilder AddSnapTrace(this ILoggingBuilder builder, Action<LoggerOptions> configure)
    {
        LoggerOptions options = new();

        configure?.Invoke(options);

        builder.AddProvider(new LoggerProvider(options));

        return builder;
    }
}
