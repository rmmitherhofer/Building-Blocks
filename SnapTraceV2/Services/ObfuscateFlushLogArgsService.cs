using SnapTraceV2.Args;
using SnapTraceV2.Models.Http;
using System.Linq.Expressions;

namespace SnapTraceV2.Services;

internal class ObfuscateFlushLogArgsService
{
    internal const string Placeholder = "***obfuscated***";

    private readonly IObfuscationService _obfuscationService;
    public ObfuscateFlushLogArgsService(IObfuscationService obfuscationService) => _obfuscationService = obfuscationService;

    public void Obfuscate(FlushLogArgs flushLogArgs)
    {
        ArgumentNullException.ThrowIfNull(flushLogArgs, nameof(FlushLogArgs));

        if (_obfuscationService is null) return;

        RequestProperties requestProperties = CreateRequestProperties(flushLogArgs);
        ResponseProperties responseProperties = CreateResponseProperties(flushLogArgs);

        flushLogArgs.HttpProperties.Request.SetProperties(requestProperties);
        flushLogArgs.HttpProperties.Response.SetProperties(responseProperties);
    }

    private RequestProperties CreateRequestProperties(FlushLogArgs flushLogArgs)
    {
        var headers = Obfuscate(flushLogArgs.HttpProperties.Request.Properties.Headers, GetPropertyName(p => p.HttpProperties.Request.Properties.Headers));
        var cookies = Obfuscate(flushLogArgs.HttpProperties.Request.Properties.Cookies, GetPropertyName(p => p.HttpProperties.Request.Properties.Cookies));
        var formData = Obfuscate(flushLogArgs.HttpProperties.Request.Properties.FormData, GetPropertyName(p => p.HttpProperties.Request.Properties.FormData));
        var serverVariables = Obfuscate(flushLogArgs.HttpProperties.Request.Properties.ServerVariables, GetPropertyName(p => p.HttpProperties.Request.Properties.ServerVariables));
        var claims = Obfuscate(flushLogArgs.HttpProperties.Request.Properties.Claims, GetPropertyName(p => p.HttpProperties.Request.Properties.Claims));

        return new (new RequestProperties.CreateOptions
        {
            Headers = headers,
            Cookies = cookies,
            QueryString = flushLogArgs.HttpProperties.Request.Properties.QueryString,
            FormData = formData,
            ServerVariables = serverVariables,
            Claims = claims,
            InputStream = flushLogArgs.HttpProperties.Request.Properties.InputStream
        });
    }

    private ResponseProperties CreateResponseProperties(FlushLogArgs flushLogArgs)
    {
        var headers = Obfuscate(flushLogArgs.HttpProperties.Response.Properties.Headers, GetPropertyName(p => p.HttpProperties.Response.Properties.Headers));

        return new(new ResponseProperties.CreateOptions
        {
            ContentLength = flushLogArgs.HttpProperties.Response.Properties.ContentLength,
            Headers = headers
        });
    }

    private List<KeyValuePair<string, string?>> Obfuscate(IEnumerable<KeyValuePair<string, string?>> values, string propertyName)
    {
        //METODO PARA OFUSCAR VALORES, COMO SENHAS

        ArgumentNullException.ThrowIfNull(values, nameof(IEnumerable<KeyValuePair<string, string?>>));
        ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));


        List<KeyValuePair<string, string?>> result = [];

        foreach (var item in values)
        {
            bool shouldObfuscate = _obfuscationService.ShouldObfuscate(item.Key, item.Value, propertyName);
            if (shouldObfuscate)
                result.Add(new (item.Key, Placeholder));
            else
                result.Add(item);
        }

        return result;
    }

    internal string? GetPropertyName(Expression<Func<FlushLogArgs, object>> expression)
    {
        var body = expression.ToString();

        body = body[(body.IndexOf('.') + 1)..];
        body = body.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

        return body;
    }
}
