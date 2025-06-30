using Api.Responses;
using Common.Exceptions;
using Common.Extensions;
using Common.Notifications.Interfaces;
using Common.Notifications.Messages;
using Logs.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace Common.Http;

public abstract class HttpService
{
    protected ILogger _logger;
    protected HttpClient _httpClient;    
    private Stopwatch _stopwatch;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly INotificationHandler _notification;

    public const string CONTENT_TYPE = "application/json";

    protected HttpService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, INotificationHandler notification, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient, nameof(HttpClient));
        ArgumentNullException.ThrowIfNull(httpContextAccessor, nameof(IHttpContextAccessor));
        ArgumentNullException.ThrowIfNull(logger, logger.GetType().Name);

        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _notification = notification;
        _logger = logger;
    }

    protected Task<HttpResponseMessage> GetAsync(string uri)
    {
        AddDefaultHeaders();

        LogRequest(HttpMethod.Get.Method, new Uri(_httpClient.BaseAddress! + uri));

        var response = _httpClient.GetAsync(uri);

        LogResponse(response.Result);

        return response;
    }
    protected HttpResponseMessage Get(string uri)
    {
        AddDefaultHeaders();

        LogRequest(HttpMethod.Get.Method, new Uri(_httpClient.BaseAddress! + uri));

        var response = _httpClient.GetAsync(uri).Result;

        LogResponse(response);

        return response;
    }
    protected Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
    {
        AddDefaultHeaders();

        LogRequest(HttpMethod.Post.Method, new Uri(_httpClient.BaseAddress! + uri), content);

        var response = _httpClient.PostAsync(uri, content);

        LogResponse(response.Result);

        return response;
    }
    protected HttpResponseMessage Post(string uri, HttpContent content)
    {
        AddDefaultHeaders();

        LogRequest(HttpMethod.Post.Method, new Uri(_httpClient.BaseAddress! + uri), content);

        var response = _httpClient.PostAsync(uri, content).Result;

        LogResponse(response);

        return response;
    }
    protected Task<HttpResponseMessage> PutAsync(string uri, HttpContent content)
    {
        AddDefaultHeaders();

        LogRequest(HttpMethod.Put.Method, new Uri(_httpClient.BaseAddress! + uri));

        var response = _httpClient.PutAsync(uri, content);

        LogResponse(response.Result);

        return response;
    }
    protected HttpResponseMessage Put(string uri, HttpContent content)
    {
        AddDefaultHeaders();

        LogRequest(HttpMethod.Put.Method, new Uri(_httpClient.BaseAddress! + uri), content);

        var response = _httpClient.PutAsync(uri, content).Result;

        LogResponse(response);

        return response;
    }
    protected Task<HttpResponseMessage> PatchAsync(string uri, HttpContent content)
    {
        AddDefaultHeaders();

        LogRequest(HttpMethod.Patch.Method, new Uri(_httpClient.BaseAddress! + uri));

        var response = _httpClient.PatchAsync(uri, content);

        LogResponse(response.Result);

        return response;
    }
    protected HttpResponseMessage Patch(string uri, HttpContent content)
    {
        AddDefaultHeaders();

        LogRequest(HttpMethod.Patch.Method, new Uri(_httpClient.BaseAddress! + uri), content);

        var response = _httpClient.PatchAsync(uri, content).Result;

        LogResponse(response);

        return response;
    }
    protected Task<HttpResponseMessage> DeleteAsync(string uri)
    {
        AddDefaultHeaders();

        LogRequest(HttpMethod.Delete.Method, new Uri(_httpClient.BaseAddress! + uri));

        var response = _httpClient.DeleteAsync(uri);

        LogResponse(response.Result);

        return response;
    }
    protected HttpResponseMessage Delete(string uri)
    {
        AddDefaultHeaders();

        LogRequest(HttpMethod.Delete.Method, new Uri(_httpClient.BaseAddress! + uri));

        var response = _httpClient.DeleteAsync(uri).Result;

        LogResponse(response);

        return response;
    }

    #region Serialization
    protected StringContent SerializeContent(object data)
    {
        var content = JsonConvert.SerializeObject(data);

        return new(content, Encoding.UTF8, CONTENT_TYPE);
    }
    protected static async Task<TResponse?> DeserializeObjectResponseAsync<TResponse>(HttpResponseMessage httpResponse)
        => JsonConvert.DeserializeObject<TResponse>(await httpResponse.Content.ReadAsStringAsync());

    protected static TResponse? DeserializeObjectResponse<TResponse>(HttpResponseMessage httpResponse)
        => JsonConvert.DeserializeObject<TResponse>(httpResponse.Content.ReadAsStringAsync().Result);
    #endregion

    #region Validation
    protected static bool ResponseHasErrors(HttpResponseMessage response, bool throwException = true)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.Forbidden:
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.BadRequest:
                return true;
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.InternalServerError:
                if (throwException)
                    throw new CustomHttpRequestException(response.StatusCode, $"{response.RequestMessage.Method} - {response.RequestMessage.RequestUri} - {(int)response.StatusCode} - {response.StatusCode}");

                return true;
        }

        try
        {
            response.EnsureSuccessStatusCode();
            return false;
        }
        catch
        {
            return true;
        }
    }
    protected TResponse ValidateAndReturn<TResponse>(HttpResponseMessage response)
    {
        ValidateResponse(response);

        return DeserializeObjectResponse<TResponse>(response);
    }
    protected void ValidateResponse(HttpResponseMessage response, bool throwException = false)
    {
        if (ResponseHasErrors(response, throwException))
        {
            var errorResponse = DeserializeObjectResponse<DetailsResponse>(response);

            switch (response.StatusCode)
            {
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.BadGateway:
                    foreach (var issue in errorResponse.Issues.Where(x => x.Type == IssuerResponseType.Error))
                    {
                        foreach (var error in issue.Details)                        
                            _notification.Notify(new Notification(error.LogLevel, error.Type, error.Key, error.Value, error.Detail));
                    }
                    break;
                case HttpStatusCode.BadRequest:
                    foreach (var issue in errorResponse.Issues.Where(x => x.Type == IssuerResponseType.Validation))
                    {
                        foreach (var validation in issue.Details)
                            _notification.Notify(new Notification(validation.LogLevel, validation.Type, validation.Key, validation.Value, validation.Detail));
                    }
                    break;
                default:
                    throw new CustomHttpRequestException($"{response.RequestMessage.Method} - {response.RequestMessage.RequestUri} - {(int)response.StatusCode} - {response.StatusCode}");
            }
        }
    } 
    #endregion

    #region Headers
    protected void SetBearerToken(string token)
        => _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    protected void AddHeader(string key, string value)
    {
        if(!_httpClient.DefaultRequestHeaders.Contains(key))
            _httpClient.DefaultRequestHeaders.Add(key, value);
    }
    private void AddDefaultHeaders()
    {
        AddHeaderIpAddress();
        AddHeaderUserId();
        AddHeaderCorrelationId();
        AddHeaderClientId();
        AddHeaderUserAgent();
        AddHeaderServerHostName();
    }
    private string AddHeaderIpAddress()
    {
        if (_httpContextAccessor is null) return string.Empty;

        var ip = _httpContextAccessor.HttpContext?.GetIpAddress();

        if (!string.IsNullOrEmpty(ip))
            AddHeader(HttpContextExtensions.FORWARDED, ip);

        return ip ?? string.Empty;
    }
    private string AddHeaderUserId()
    {
        if (_httpContextAccessor is null) return string.Empty;

        var userId = _httpContextAccessor.HttpContext?.GetUserId();

        if (!string.IsNullOrEmpty(userId))
            AddHeader(HttpContextExtensions.USER_ID, userId);

        return userId ?? string.Empty;
    }
    private string AddHeaderCorrelationId()
    {
        if (_httpContextAccessor is null) return string.Empty;

        var correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId();

        if(string.IsNullOrEmpty(correlationId))
            correlationId = _httpContextAccessor.HttpContext?.GetRequestId();

        if (!string.IsNullOrEmpty(correlationId))
            AddHeader(HttpContextExtensions.CORRELATION_ID, correlationId);

        return correlationId ?? string.Empty;
    }
    private string AddHeaderClientId()
    {
        if (_httpContextAccessor is null) return string.Empty;

        var clientId = _httpContextAccessor.HttpContext?.GetClientId();

        clientId = string.Join(';', clientId, Assembly.GetEntryAssembly().GetName().Name);

        if (!string.IsNullOrEmpty(clientId))
            AddHeader(HttpContextExtensions.CLIENT_ID, clientId);

        return clientId ?? string.Empty;
    }
    private string AddHeaderUserAgent()
    {
        if (_httpContextAccessor is null) return string.Empty;

        var userAgent = _httpContextAccessor.HttpContext?.GetUserAgent();

        if (!string.IsNullOrEmpty(userAgent))
            AddHeader(HttpContextExtensions.USER_AGENT, userAgent);

        return userAgent ?? string.Empty;
    }
    private string AddHeaderServerHostName()
    {
        if (_httpContextAccessor is null) return string.Empty;

        var podeName = Dns.GetHostName();

        if (!string.IsNullOrEmpty(podeName))
            AddHeader(HttpContextExtensions.POD_NAME, podeName);

        return podeName ?? string.Empty;
    }
    #endregion

    #region Logs
    private void LogRequest(string httpMehod, Uri uri, HttpContent? content = null)
    {
        _stopwatch = new();
        _stopwatch.Start();

        var headersJson = string.Empty;
        var contentJson = string.Empty;

        headersJson = $"|Headers:{JsonConvert.SerializeObject(_httpClient.DefaultRequestHeaders)}";

        if (content is not null)
            contentJson = $"|Content:{content.ReadAsStringAsync().Result}";

        _logger.LogInfo($"Start processing HTTP request {httpMehod} {uri}{headersJson}{contentJson}");
    }
    private void LogResponse(HttpResponseMessage response)
    {
        _stopwatch.Stop();

        string message = $"End processing HTTP request {response?.RequestMessage?.Method} {response?.RequestMessage?.RequestUri} after {_stopwatch.ElapsedMilliseconds.GetTime()} - {response.StatusCode}";

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
            case HttpStatusCode.Created:
            case HttpStatusCode.Accepted:
            case HttpStatusCode.NoContent:
            case HttpStatusCode.Continue:
            case HttpStatusCode.ResetContent:
            case HttpStatusCode.PartialContent:
            case HttpStatusCode.MultiStatus:
            case HttpStatusCode.AlreadyReported:
            case HttpStatusCode.IMUsed:
                _logger.LogInfo(message);
                break;
            case HttpStatusCode.BadRequest:
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.PaymentRequired:
            case HttpStatusCode.Forbidden:
            case HttpStatusCode.NotFound:
            case HttpStatusCode.MethodNotAllowed:
            case HttpStatusCode.NotAcceptable:
            case HttpStatusCode.ProxyAuthenticationRequired:
            case HttpStatusCode.RequestTimeout:
            case HttpStatusCode.Conflict:
            case HttpStatusCode.Gone:
            case HttpStatusCode.LengthRequired:
            case HttpStatusCode.PreconditionFailed:
            case HttpStatusCode.RequestEntityTooLarge:
            case HttpStatusCode.RequestUriTooLong:
            case HttpStatusCode.UnsupportedMediaType:
            case HttpStatusCode.RequestedRangeNotSatisfiable:
            case HttpStatusCode.ExpectationFailed:
            case HttpStatusCode.MisdirectedRequest:
            case HttpStatusCode.UnprocessableEntity:
            case HttpStatusCode.Locked:
            case HttpStatusCode.FailedDependency:
            case HttpStatusCode.UpgradeRequired:
            case HttpStatusCode.PreconditionRequired:
            case HttpStatusCode.TooManyRequests:
            case HttpStatusCode.RequestHeaderFieldsTooLarge:
            case HttpStatusCode.UnavailableForLegalReasons:
                _logger.LogWarn(message);
                break;
            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.NotImplemented:
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.ServiceUnavailable:
            case HttpStatusCode.GatewayTimeout:
            case HttpStatusCode.HttpVersionNotSupported:
            case HttpStatusCode.VariantAlsoNegotiates:
            case HttpStatusCode.InsufficientStorage:
            case HttpStatusCode.LoopDetected:
            case HttpStatusCode.NotExtended:
            case HttpStatusCode.NetworkAuthenticationRequired:
                _logger.LogError(message);
                break;
            default:
                _logger.LogCrit(message);
                break;
        }
    } 
    #endregion
}