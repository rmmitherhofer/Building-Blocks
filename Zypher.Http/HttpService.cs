using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using Zypher.Extensions.Core;
using Zypher.Http.Exceptions;
using Zypher.Http.Extensions;
using Zypher.Logs.Extensions;
using Zypher.Notifications.Interfaces;
using Zypher.Notifications.Messages;
using Zypher.Responses;

namespace Zypher.Http;

/// <summary>
/// Abstract base class for making HTTP requests with integrated logging, header injection, 
/// response validation, and support for both synchronous and asynchronous execution of all major HTTP verbs.
/// </summary>
public abstract class HttpService
{
    /// <summary>
    /// Stores the URI template used to identify the logical route in logs.
    /// </summary>
    private string _templateUri = string.Empty;
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
    /// Indicates whether detailed request and response logging (headers and body) is enabled.
    /// </summary>
    private bool IsDetailedLoggingEnabled = false;
    /// <summary>
    /// Notification handler for capturing validation or API errors.
    /// </summary>
    protected readonly INotificationHandler _notification;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpService"/> class.
    /// </summary>
    /// <param name="httpClient">An <see cref="HttpClient"/> instance used to send HTTP requests.</param>
    /// <param name="notification">An implementation of <see cref="INotificationHandler"/> for recording errors and issues.</param>
    /// <param name="logger">An <see cref="ILogger"/> instance for writing logs related to requests and responses.</param>
    protected HttpService(HttpClient httpClient, INotificationHandler notification, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient, nameof(HttpClient));
        ArgumentNullException.ThrowIfNull(logger, logger.GetType().Name);

        _httpClient = httpClient;
        _notification = notification;
        _logger = logger;
    }

    #region GET
    /// <summary>
    /// Sends an asynchronous GET request to the specified URI and sets a template identifier for contextual logging.
    /// </summary>
    /// <param name="uri">A tuple containing the template identifier and the request URI.</param>
    /// <returns>The HTTP response message.</returns>

    protected Task<HttpResponseMessage> GetAsync((string template, string uri) uri)
    {
        _templateUri = uri.template;
        _httpClient.AddHeaderRequestTemplate(uri.template);

        return GetAsync(uri.uri);
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
    /// Sends a synchronous GET request to the specified URI and sets a template identifier for contextual logging.
    /// </summary>

    protected HttpResponseMessage Get((string template, string uri) uri)
    {
        _templateUri = uri.template;
        _httpClient.AddHeaderRequestTemplate(uri.template);

        return Get(uri.uri);
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
    #endregion

    #region POST
    /// <summary>
    /// Sends an asynchronous POST request to the specified URI and sets a template identifier for contextual logging.
    /// </summary>
    /// <param name="uri">A tuple containing the template identifier and the request URI.</param>
    /// <returns>The HTTP response message.</returns>
    protected Task<HttpResponseMessage> PostAsync((string template, string uri) uri, HttpContent content)
    {
        _templateUri = uri.template;
        _httpClient.AddHeaderRequestTemplate(uri.template);

        return PostAsync(uri.uri, content);
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
    /// Sends a synchronous POST request to the specified URI and sets a template identifier for contextual logging.
    /// </summary>
    protected HttpResponseMessage Post((string template, string uri) uri, HttpContent content)
    {
        _templateUri = uri.template;
        _httpClient.AddHeaderRequestTemplate(uri.template);

        return Post(uri.uri, content);
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
    #endregion

    #region PUT
    /// <summary>
    /// Sends an asynchronous PUT request to the specified URI and sets a template identifier for contextual logging.
    /// </summary>
    /// <param name="uri">A tuple containing the template identifier and the request URI.</param>
    /// <returns>The HTTP response message.</returns>
    protected Task<HttpResponseMessage> PutAsync((string template, string uri) uri, HttpContent content)
    {
        _templateUri = uri.template;
        _httpClient.AddHeaderRequestTemplate(uri.template);

        return PutAsync(uri.uri, content);
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
    /// Sends a synchronous PUT request to the specified URI and sets a template identifier for contextual logging.
    /// </summary>
    protected HttpResponseMessage Put((string template, string uri) uri, HttpContent content)
    {
        _templateUri = uri.template;
        _httpClient.AddHeaderRequestTemplate(uri.template);

        return Put(uri.uri, content);
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

    #endregion

    #region PATCH
    /// <summary>
    /// Sends an asynchronous PATCH request to the specified URI and sets a template identifier for contextual logging.
    /// </summary>
    /// <param name="uri">A tuple containing the template identifier and the request URI.</param>
    /// <returns>The HTTP response message.</returns>
    protected Task<HttpResponseMessage> PatchAsync((string template, string uri) uri, HttpContent content)
    {
        _templateUri = uri.template;
        _httpClient.AddHeaderRequestTemplate(uri.template);

        return PatchAsync(uri.uri, content);
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
    /// Sends a synchronous PATCH request to the specified URI and sets a template identifier for contextual logging.
    /// </summary>
    protected HttpResponseMessage Patch((string template, string uri) uri, HttpContent content)
    {
        _templateUri = uri.template;
        _httpClient.AddHeaderRequestTemplate(uri.template);

        return Patch(uri.uri, content);
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
    #endregion

    #region DELETE
    /// <summary>
    /// Sends an asynchronous DELETE request to the specified URI and sets a template identifier for contextual logging.
    /// </summary>
    /// <param name="uri">A tuple containing the template identifier and the request URI.</param>
    /// <returns>The HTTP response message.</returns>
    protected Task<HttpResponseMessage> DeleteAsync((string template, string uri) uri)
    {
        _templateUri = uri.template;
        _httpClient.AddHeaderRequestTemplate(uri.template);

        return DeleteAsync(uri.uri);
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
    /// Sends a synchronous DELETE request to the specified URI and sets a template identifier for contextual logging.
    /// </summary>
    protected HttpResponseMessage Delete((string template, string uri) uri)
    {
        _templateUri = uri.template;
        _httpClient.AddHeaderRequestTemplate(uri.template);

        return Delete(uri.uri);
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

    #endregion

    #region Validation

    /// <summary>
    /// Validates the HTTP response and deserializes its content to a specified type if no critical errors are found.
    /// </summary>
    /// <typeparam name="TResponse">The type to deserialize the HTTP response content into.</typeparam>
    /// <param name="response">The HTTP response message to validate and process.</param>
    /// <returns>The deserialized response object, or null if response is empty.</returns>
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
                    _notification.Notify(new Notification(error.LogLevel ?? LogLevel.None, error.Type, error.Key, error.Value, error.Detail));
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
    /// Enables verbose logging of HTTP request and response, including headers and body content.
    /// </summary>
    protected void EnableLogHeadersAndBody() => IsDetailedLoggingEnabled = true;

    #region Logs

    /// <summary>
    /// Logs the start of an HTTP request including method, URI, headers, and optional content.
    /// </summary>
    protected void LogRequest(string httpMehod, Uri uri, HttpContent? content = null)
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
        _logger.LogInfo($"Start processing HTTP request {httpMehod} {(string.IsNullOrEmpty(_templateUri) ? uri : _httpClient.BaseAddress + _templateUri)}{headersJson}{contentJson}");
    }

    /// <summary>
    /// Logs the end of an HTTP request including method, URI, elapsed time and response status.
    /// </summary>
    protected void LogResponse(HttpResponseMessage response)
    {
        _stopwatch.Stop();

        string message = $"End processing HTTP request {response?.RequestMessage?.Method} {(string.IsNullOrEmpty(_templateUri) ? response?.RequestMessage?.RequestUri : _httpClient.BaseAddress + _templateUri)} after {_stopwatch.GetFormattedTime()} - {(int)response.StatusCode}-{response.StatusCode}";

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
