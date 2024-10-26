using Api.Responses;
using Common.Exceptions;
using Common.Extensions;
using Common.Notifications.Interfaces;
using Common.Notifications.Messages;
using Logs.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Common.Http;

public abstract class HttpService
{
    protected HttpClient _httpClient;
    private Stopwatch _stopwatch;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly INotificationHandler _notification;

    protected HttpService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, INotificationHandler notification)
    {
        ArgumentNullException.ThrowIfNull(httpClient, nameof(HttpClient));
        ArgumentNullException.ThrowIfNull(httpContextAccessor, nameof(IHttpContextAccessor));

        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _notification = notification;
    }

    protected Task<HttpResponseMessage> GetAsync(string uri)
    {
        AddDefaultHeaders();

        _stopwatch = new();

        _stopwatch.Start();

        var response = _httpClient.GetAsync(uri);

        _stopwatch.Stop();

        Log(response.Result);

        return response;
    }
    protected HttpResponseMessage Get(string uri)
    {

        AddDefaultHeaders();

        _stopwatch = new();

        _stopwatch.Start();

        var response = _httpClient.GetAsync(uri).Result;

        _stopwatch.Stop();

        Log(response);

        return response;

    }
    protected Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
    {
        AddDefaultHeaders();

        _stopwatch = new();

        _stopwatch.Start();

        var response = _httpClient.PostAsync(uri, content);

        _stopwatch.Stop();

        Log(response.Result);

        return response;
    }
    protected HttpResponseMessage Post(string uri, HttpContent content)
    {
        AddDefaultHeaders();

        _stopwatch = new();

        _stopwatch.Start();

        var response = _httpClient.PostAsync(uri, content).Result;

        _stopwatch.Stop();

        Log(response);

        return response;
    }
    protected Task<HttpResponseMessage> PutAsync(string uri, HttpContent content)
    {
        AddDefaultHeaders();

        _stopwatch = new();

        _stopwatch.Start();

        var response = _httpClient.PutAsync(uri, content);

        _stopwatch.Stop();

        Log(response.Result);

        return response;
    }
    protected HttpResponseMessage Put(string uri, HttpContent content)
    {
        AddDefaultHeaders();

        _stopwatch = new();

        _stopwatch.Start();

        var response = _httpClient.PutAsync(uri, content).Result;

        _stopwatch.Stop();

        Log(response);

        return response;
    }
    protected Task<HttpResponseMessage> DeleteAsync(string uri)
    {
        AddDefaultHeaders();

        _stopwatch = new();

        _stopwatch.Start();

        var response = _httpClient.DeleteAsync(uri);

        _stopwatch.Stop();

        Log(response.Result);

        return response;
    }
    protected HttpResponseMessage Delete(string uri)
    {
        AddDefaultHeaders();

        _stopwatch = new();

        _stopwatch.Start();

        var response = _httpClient.DeleteAsync(uri).Result;

        _stopwatch.Stop();

        Log(response);

        return response;
    }
    protected StringContent SerializeContent(object data)
    {
        var content = JsonConvert.SerializeObject(data);

        return new(content, Encoding.UTF8, "application/json");
    }
    protected static async Task<TResponse?> DeserializeObjectResponseAsync<TResponse>(HttpResponseMessage httpResponse)
        => JsonConvert.DeserializeObject<TResponse>(await httpResponse.Content.ReadAsStringAsync());
    protected static TResponse? DeserializeObjectResponse<TResponse>(HttpResponseMessage httpResponse)
        => JsonConvert.DeserializeObject<TResponse>(httpResponse.Content.ReadAsStringAsync().Result);
    protected static bool ResponseHasErrors(HttpResponseMessage response, bool throwException = true)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.NotFound:
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
            switch (response.StatusCode)
            {
                case HttpStatusCode.InternalServerError:
                    var errorResponse = DeserializeObjectResponse<ErrorResponse>(response);
                    foreach (var error in errorResponse.Errors)
                    {
                        _notification.Notify(new Notification(error.LogLevel, error.Type, error.Key, error.Value));
                    }
                    break;
                case HttpStatusCode.BadRequest:
                    var validationResponse = DeserializeObjectResponse<ValidationResponse>(response);
                    foreach (var validation in validationResponse.Validations)
                    {
                        _notification.Notify(new Notification(validation.LogLevel, validation.Type, validation.Key, validation.Value));
                    }
                    break;
                default:
                    throw new CustomHttpRequestException($"{response.RequestMessage.Method} - {response.RequestMessage.RequestUri} - {(int)response.StatusCode} - {response.StatusCode}");
            }
        }
    }
    protected void SetBearerToken(string token) 
        => _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    private void AddDefaultHeaders()
    {
        AddHeaderIpAddress();
        AddHeaderUserId();
        AddHeaderSessionId();
    }
    private string AddHeaderIpAddress()
    {
        if (_httpContextAccessor is null) return string.Empty;

        var ip = _httpContextAccessor.HttpContext?.GetIpAddress();

        if (!string.IsNullOrEmpty(ip))
            _httpClient.DefaultRequestHeaders?.Add(HttpContextExtensions.USER_IP_ADDRESS, ip);

        return ip ?? string.Empty;
    }
    private string AddHeaderUserId()
    {
        if (_httpContextAccessor is null) return string.Empty;

        var userId = _httpContextAccessor.HttpContext?.GetUserId();

        if (!string.IsNullOrEmpty(userId))
            _httpClient.DefaultRequestHeaders?.Add(HttpContextExtensions.USER_ID, userId);

        return userId ?? string.Empty;
    }
    private string AddHeaderSessionId()
    {
        if (_httpContextAccessor is null) return string.Empty;

        var sessionId = _httpContextAccessor.HttpContext?.GetSessionId();

        if (!string.IsNullOrEmpty(sessionId))
            _httpClient.DefaultRequestHeaders?.Add(HttpContextExtensions.USER_SESSION_ID, sessionId);

        return sessionId ?? string.Empty;
    }
    private void Log(HttpResponseMessage response)
    {
        ConsoleLog.LogInfo($"Start processing HTTP request {response?.RequestMessage?.Method} {response?.RequestMessage?.RequestUri}", _httpContextAccessor?.HttpContext);
        ConsoleLog.LogInfo($"End processing HTTP request {response?.RequestMessage?.Method} {response?.RequestMessage?.RequestUri} after {_stopwatch.ElapsedMilliseconds.GetTime()}", _httpContextAccessor?.HttpContext);
    }
}