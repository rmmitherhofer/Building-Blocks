using Microsoft.AspNetCore.Http;
using SnapTraceV2.Services;
using System.Collections.Concurrent;

namespace SnapTraceV2.Factories;

internal class LoggerFactory : ILoggerFactory
{
    public const string DictionaryKey = "SnapTrace-Loggers";

    internal readonly IHttpContextAccessor _httpContextAccessor;
    public LoggerFactory(IHttpContextAccessor? httpContextAccessor = null) => _httpContextAccessor = httpContextAccessor ?? new HttpContextAccessor();

    public LoggerService Get(string? category = null, string? url = null)
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null) return new(category, url);

        return GetInstance(httpContext, category);
    }

    public IEnumerable<LoggerService> GetAll()
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null) return [];

        return GetAll(httpContext);
    }

    internal LoggerService GetInstance(HttpContext context, string? categoryName = null)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(HttpContext));

        ConcurrentDictionary<string, LoggerService>? container = null;

        if (context.Items.ContainsKey(DictionaryKey))
        {
            container = context.Items[DictionaryKey] as ConcurrentDictionary<string, LoggerService>;
        }
        else
        {
            container = new ConcurrentDictionary<string, LoggerService>();

            context.Items.Add(DictionaryKey, container);
        }

        LoggerService logger = new(categoryName);

        logger.DataContainer.LoggerProperties.IsManagedByHttpRequest = true;

        return container!.GetOrAdd(logger.Category, (key) => logger);
    }

    internal IEnumerable<LoggerService> GetAll(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!context.Items.ContainsKey(DictionaryKey)) return [];

        ConcurrentDictionary<string, LoggerService>? container = context.Items[DictionaryKey] as ConcurrentDictionary<string, LoggerService>;

        List<LoggerService> loggers = [];

        foreach (string key in container!.Keys)
        {
            if (container.TryGetValue(key, out LoggerService? logger) && logger != null)
                loggers.Add(logger);
        }

        return loggers;
    }
}
