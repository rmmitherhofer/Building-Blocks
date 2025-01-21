using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using SnapTraceV2.Extensions;
using SnapTraceV2.Helpers;
using SnapTraceV2.Models.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace SnapTraceV2.Factories;

internal class HttpRequestFactory
{
    private const string X_SnapTraceSessionId = "X-SnapTraceSessionId";

    public static HttpRequest Create(Microsoft.AspNetCore.Http.HttpRequest httpRequest)
    {
        ArgumentNullException.ThrowIfNull(httpRequest, nameof(Microsoft.AspNetCore.Http.HttpRequest));

        var result = GetHttpRequest(httpRequest);

        var propertiesOptions = GetRequestProperties(httpRequest);

        if (httpRequest.HasFormContentType && SnapTraceOptionsConfiguration.Options.Handlers.ShouldLogFormData.Invoke(result))
            propertiesOptions.FormData = httpRequest.Form.ToKeyValuePair();

        if (InternalHelper.CanReadRequestInputStream(propertiesOptions.Headers) && SnapTraceOptionsConfiguration.Options.Handlers.ShouldLogInputStream.Invoke(result))
            propertiesOptions.InputStream = InternalHelper.WrapInTryCatch(() => ModuleInitializer.ReadInputStreamProvider.ReadInputStream(httpRequest));

        result.SetProperties(new RequestProperties(propertiesOptions));

        return result;
    }

    private static HttpRequest GetHttpRequest(Microsoft.AspNetCore.Http.HttpRequest httpRequest)
    {
        Session? session = InternalHelper.WrapInTryCatch(() => GetSession(httpRequest));

        session ??= new Session();

        bool isAuthenticated = httpRequest.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        return new(new HttpRequest.CreateOptions
        {
            Url = new Uri(GetDisplayUrl(httpRequest)),
            HttpMethod = httpRequest.Method,
            UserAgent = GetHeaderValue(httpRequest.Headers, HeaderNames.UserAgent),
            HttpReferer = GetHeaderValue(httpRequest.Headers, HeaderNames.Referer),
            RemoteAddress = httpRequest.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
            MachineName = InternalHelper.GetMachineName(),
            IsNewSession = session.IsNewSession,
            SessionId = session.SessionId,
            IsAuthenticated = isAuthenticated
        });
    }
    private static Session GetSession(Microsoft.AspNetCore.Http.HttpRequest httpRequest)
    {
        ArgumentNullException.ThrowIfNull(httpRequest);

        bool isNewSession = false;
        string? sessionId = null;

        if (httpRequest.HttpContext?.Session is not null && httpRequest.HttpContext.Session.IsAvailable)
        {
            if (httpRequest.HttpContext.Session.TryGetValue(X_SnapTraceSessionId, out var value) && value is not null)
                sessionId = Encoding.UTF8.GetString(value);

            if (string.IsNullOrEmpty(sessionId) || string.Equals(sessionId, httpRequest.HttpContext.Session.Id, StringComparison.OrdinalIgnoreCase))
            {
                isNewSession = true;
                var sessionIdBytes = Encoding.UTF8.GetBytes(httpRequest.HttpContext.Session.Id);
                httpRequest.HttpContext.Session.Set(X_SnapTraceSessionId, sessionIdBytes);
            }

            sessionId = httpRequest.HttpContext.Session.Id;
        }

        return new()
        {
            IsNewSession = isNewSession,
            SessionId = sessionId
        };
    }

    private static RequestProperties.CreateOptions GetRequestProperties(Microsoft.AspNetCore.Http.HttpRequest httpRequest)
        => new()
        {
            Cookies = httpRequest.Cookies.ToKeyValuePair(),
            Headers = httpRequest.Headers.ToKeyValuePair(),
            QueryString = httpRequest.Query.ToKeyValuePair(),
            Claims = GetClaims(httpRequest)
        };

    private static string GetDisplayUrl(Microsoft.AspNetCore.Http.HttpRequest request)
    {
        string value = request.Host.Value;
        string value2 = request.PathBase.Value ?? string.Empty;
        string value3 = request.Path.Value ?? string.Empty;
        string value4 = request.QueryString.Value ?? string.Empty;
        return new StringBuilder(request.Scheme.Length + "://".Length + value.Length + value2.Length + value3.Length + value4.Length).Append(request.Scheme).Append("://").Append(value)
            .Append(value2)
            .Append(value3)
            .Append(value4)
            .ToString();
    }

    private static string? GetHeaderValue(IDictionary<string, StringValues> requestHeaders, string headerName)
    {
        if (requestHeaders is null) return null;

        if (requestHeaders.TryGetValue(headerName, out StringValues value))
            return value.ToString();

        return null;
    }

    private static IEnumerable<KeyValuePair<string, string>> GetClaims(Microsoft.AspNetCore.Http.HttpRequest httpRequest)
    {
        ArgumentNullException.ThrowIfNull(httpRequest);

        IIdentity? identity = httpRequest.HttpContext?.User?.Identity;

        if (identity is null) return [];

        if (identity is not ClaimsIdentity) return [];

        ClaimsIdentity? claimsIdentity = identity as ClaimsIdentity;

        return claimsIdentity!.ToKeyValuePair();
    }

    class Session
    {
        public string? SessionId { get; set; }
        public bool IsNewSession { get; set; }
    }
}