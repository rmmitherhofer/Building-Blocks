using Api.Responses;
using Common.Exceptions;
using Common.Extensions;
using Common.Http.Extensions;
using Common.Logs.Extensions;
using Common.Notifications.Interfaces;
using Common.Notifications.Messages;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;

namespace Common.Http;
/// <summary>
/// Base class for making HTTP requests with built-in logging, header configuration, and response validation.
/// Supports synchronous and asynchronous operations for all main HTTP verbs.
/// </summary>
public abstract class HttpService
{
    /// <summary>
    /// Logger for logging request and response inf
    /// </summary>
    protected ILogger _logger;
    /// <summary>
    /// HTTP client instance used for sending requests.
    /// </summary>
    protected HttpClient _httpClient;
    /// <summary>
    /// Stopwatch for measuring request duration.
    /// </summary>
    private Stopwatch _stopwatch;
    /// <summary>
    /// Flag indicating whether to log headers and body content.
    /// </summary>
    private bool IsDetailedLoggingEnabled = false;
    /// <summary>
    /// Notification handler for capturing validation or API errors.
    /// </summary>
    protected readonly INotificationHandler _notification;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpService"/> class.
    /// </summary>
    /// <param name="httpClient">HttpClient to be used for requests.</param>
    /// <param name="notification">Notification handler for storing notifications.</param>
    /// <param name="logger">Logger instance for logging request and response details.</param>
    protected HttpService(HttpClient httpClient, INotificationHandler notification, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient, nameof(HttpClient));
        ArgumentNullException.ThrowIfNull(logger, logger.GetType().Name);

        _httpClient = httpClient;
        _notification = notification;
        _logger = logger;
    }

    /// <summary>
    /// Sends a GET request asynchronously to the specified URI.
    /// </summary>
    protected Task<HttpResponseMessage> GetAsync(string uri)
    {
        _httpClient.AddDefaultHeaders();

        LogRequest(HttpMethod.Get.Method, new Uri(_httpClient.BaseAddress! + uri));

        var response = _httpClient.GetAsync(uri);

        LogResponse(response.Result);

        return response;
    }

    /// <summary>
    /// Sends a GET request synchronously to the specified URI.
    /// </summary>
    protected HttpResponseMessage Get(string uri)
    {
        _httpClient.AddDefaultHeaders();

        LogRequest(HttpMethod.Get.Method, new Uri(_httpClient.BaseAddress! + uri));

        var response = _httpClient.GetAsync(uri).Result;

        LogResponse(response);

        return response;
    }

    /// <summary>
    /// Sends a POST request asynchronously to the specified URI with the provided content.
    /// </summary>
    protected Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
    {
        _httpClient.AddDefaultHeaders();

        LogRequest(HttpMethod.Post.Method, new Uri(_httpClient.BaseAddress! + uri), content);

        var response = _httpClient.PostAsync(uri, content);

        LogResponse(response.Result);

        return response;
    }

    /// <summary>
    /// Sends a POST request synchronously to the specified URI with the provided content.
    /// </summary>
    protected HttpResponseMessage Post(string uri, HttpContent content)
    {
        _httpClient.AddDefaultHeaders();

        LogRequest(HttpMethod.Post.Method, new Uri(_httpClient.BaseAddress! + uri), content);

        var response = _httpClient.PostAsync(uri, content).Result;

        LogResponse(response);

        return response;
    }

    /// <summary>
    /// Sends a PUT request asynchronously to the specified URI with the provided content.
    /// </summary>
    protected Task<HttpResponseMessage> PutAsync(string uri, HttpContent content)
    {
        _httpClient.AddDefaultHeaders();

        LogRequest(HttpMethod.Put.Method, new Uri(_httpClient.BaseAddress! + uri));

        var response = _httpClient.PutAsync(uri, content);

        LogResponse(response.Result);

        return response;
    }

    /// <summary>
    /// Sends a PUT request synchronously to the specified URI with the provided content.
    /// </summary>
    protected HttpResponseMessage Put(string uri, HttpContent content)
    {
        _httpClient.AddDefaultHeaders();

        LogRequest(HttpMethod.Put.Method, new Uri(_httpClient.BaseAddress! + uri), content);

        var response = _httpClient.PutAsync(uri, content).Result;

        LogResponse(response);

        return response;
    }

    /// <summary>
    /// Sends a PATCH request asynchronously to the specified URI with the provided content.
    /// </summary>
    protected Task<HttpResponseMessage> PatchAsync(string uri, HttpContent content)
    {
        _httpClient.AddDefaultHeaders();

        LogRequest(HttpMethod.Patch.Method, new Uri(_httpClient.BaseAddress! + uri));

        var response = _httpClient.PatchAsync(uri, content);

        LogResponse(response.Result);

        return response;
    }

    /// <summary>
    /// Sends a PATCH request synchronously to the specified URI with the provided content.
    /// </summary>
    protected HttpResponseMessage Patch(string uri, HttpContent content)
    {
        _httpClient.AddDefaultHeaders();

        LogRequest(HttpMethod.Patch.Method, new Uri(_httpClient.BaseAddress! + uri), content);

        var response = _httpClient.PatchAsync(uri, content).Result;

        LogResponse(response);

        return response;
    }

    /// <summary>
    /// Sends a DELETE request asynchronously to the specified URI.
    /// </summary>
    protected Task<HttpResponseMessage> DeleteAsync(string uri)
    {
        _httpClient.AddDefaultHeaders();

        LogRequest(HttpMethod.Delete.Method, new Uri(_httpClient.BaseAddress! + uri));

        var response = _httpClient.DeleteAsync(uri);

        LogResponse(response.Result);

        return response;
    }

    /// <summary>
    /// Sends a DELETE request synchronously to the specified URI.
    /// </summary>
    protected HttpResponseMessage Delete(string uri)
    {
        _httpClient.AddDefaultHeaders();

        LogRequest(HttpMethod.Delete.Method, new Uri(_httpClient.BaseAddress! + uri));

        var response = _httpClient.DeleteAsync(uri).Result;

        LogResponse(response);

        return response;
    }

    #region Validation

    /// <summary>
    /// Validates the HTTP response and deserializes its content to the specified response type.
    /// </summary>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="response">The HTTP response message to validate and read.</param>
    /// <returns>The deserialized response object.</returns>
    protected async Task<TResponse?> ValidateAndReturn<TResponse>(HttpResponseMessage response)
    {
        await ValidateResponse(response);

        return await response.ReadAsAsync<TResponse>();
    }

    /// <summary>
    /// Validates the HTTP response, notifies issues, and optionally throws exceptions for unexpected status codes.
    /// </summary>
    /// <param name="response">The HTTP response message to validate.</param>
    /// <param name="throwException">Determines whether to throw an exception on unexpected status codes.</param>
    protected async Task ValidateResponse(HttpResponseMessage response, bool throwException = false)
    {
        if (response.HasErrors(throwException))
        {
            var apiResponse = await response.ReadAsAsync<ApiResponse>();

            foreach (var issue in apiResponse.Issues)
            {
                if (issue.Details?.Any() != true) continue;

                foreach (var error in issue.Details)
                    _notification.Notify(new Notification(error.LogLevel, error.Type, error.Key, error.Value, error.Detail));
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Created:
                case HttpStatusCode.Continue:
                case HttpStatusCode.ResetContent:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.PartialContent:
                case HttpStatusCode.MultiStatus:
                case HttpStatusCode.AlreadyReported:
                case HttpStatusCode.IMUsed:
                    break;
                default:
                    throw new CustomHttpRequestException($"{response.RequestMessage.Method} - {response.RequestMessage.RequestUri} - {(int)response.StatusCode} - {response.StatusCode}");
            }
        }
    }
    #endregion

    /// <summary>
    /// Enables logging of HTTP headers and body content for all requests and responses.
    /// </summary>
    protected void EnableLogHeadersAndBody() => IsDetailedLoggingEnabled = true;

    #region Logs

    /// <summary>
    /// Logs the start of an HTTP request including method, URI, headers, and optional content.
    /// </summary>
    private void LogRequest(string httpMehod, Uri uri, HttpContent? content = null)
    {
        _stopwatch = new();
        _stopwatch.Start();

        var headersJson = string.Empty;
        var contentJson = string.Empty;
        if (IsDetailedLoggingEnabled)
        {
            headersJson = $"|Headers:{_httpClient.GetHeadersJsonFormat()}";

            if (content is not null)
                contentJson = $"|Content:{content.ReadAsStringAsync().Result}";
        }
        _logger.LogInfo($"Start processing HTTP request {httpMehod} {uri}{headersJson}{contentJson}");
    }

    /// <summary>
    /// Logs the end of an HTTP request including method, URI, elapsed time and response status.
    /// </summary>
    private void LogResponse(HttpResponseMessage response)
    {
        _stopwatch.Stop();

        string message = $"End processing HTTP request {response?.RequestMessage?.Method} {response?.RequestMessage?.RequestUri} after {_stopwatch.GetTime()} - {response.StatusCode}";

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
