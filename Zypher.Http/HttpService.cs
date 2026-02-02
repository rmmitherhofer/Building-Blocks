using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using Zypher.Http.Exceptions;
using Zypher.Http.Extensions;
using Zypher.Logs.Extensions;
using Zypher.Notifications.Interfaces;
using Zypher.Notifications.Messages;
using Zypher.Responses;

namespace Zypher.Http;

/// <summary>
/// Provides a reusable base for executing HTTP requests with built-in:
/// <list type="bullet">
/// <item>Context-aware request logging</item>
/// <item>Per-request header customization</item>
/// <item>Response validation and error propagation</item>
/// <item>Execution time measurement</item>
/// </list>
/// </summary>
/// <remarks>
/// This service is designed to be inherited by API clients and ensures that:
/// <list type="bullet">
/// <item>Headers are isolated per request (thread-safe)</item>
/// <item>Request templates can be tracked for structured logging</item>
/// <item>Failures are converted into domain notifications</item>
/// </list>
/// </remarks>
public abstract class HttpService
{
    /// <summary>
    /// Stores the request template used for structured logging and route correlation.
    /// </summary>
    private static readonly HttpRequestOptionsKey<string> TemplateKey = new("__TEMPLATE_URI_KEY__");
    /// <summary>
    /// Logger responsible for request lifecycle logging.
    /// </summary>
    protected ILogger _logger;
    /// <summary>
    /// Underlying HTTP client used to perform network calls.
    /// </summary>
    protected HttpClient _httpClient;
    /// <summary>
    /// Enables detailed request and response logging including headers and payloads.
    /// </summary>
    private bool IsDetailedLoggingEnabled = false;
    /// <summary>
    /// Collects domain-level notifications produced from API responses.
    /// </summary>
    protected readonly INotificationHandler _notification;

    /// <summary>
    /// Creates a new HTTP service instance with logging and notification support.
    /// </summary>
    /// <param name="httpClient">
    /// Preconfigured <see cref="HttpClient"/> used to execute requests.
    /// </param>
    /// <param name="notification">
    /// Handler responsible for collecting API validation and error notifications.
    /// </param>
    /// <param name="logger">
    /// Logger used to record request lifecycle events.
    /// </param>
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
    /// Executes a GET request using a logical route template for structured logging.
    /// </summary>
    /// <param name="uri">
    /// Tuple containing the route template identifier and the actual request URI.
    /// </param>
    /// <param name="configureHeaders">
    /// Optional delegate for configuring request-specific headers.
    /// </param>
    /// <param name="completionOption">
    /// Controls when the HTTP operation is considered complete.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the request.
    /// </param>
    /// <returns>The HTTP response.</returns>

    protected Task<HttpResponseMessage> GetAsync((string template, string uri) uri,
        Action<HttpRequestHeaders>? configureHeaders = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uri.uri);

        request.Options.Set(TemplateKey, uri.template);
        request.AddHeaderRequestTemplate(uri.template);

        return SendAsync(request, configureHeaders, completionOption, cancellationToken);
    }

    /// <summary>
    /// Executes a GET request directly to the specified URI.
    /// </summary>
    /// <param name="uri">Target request URI.</param>
    /// <param name="configureHeaders">Optional header configuration.</param>
    /// <param name="completionOption">Completion behavior.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The HTTP response.</returns>
    protected Task<HttpResponseMessage> GetAsync(string uri,
        Action<HttpRequestHeaders>? configureHeaders = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);

        return SendAsync(request, configureHeaders, completionOption, cancellationToken);

    }
    #endregion

    #region POST
    /// <summary>
    /// Executes a POST request using a logical route template for structured logging.
    /// </summary>
    /// <param name="uri">Template and request URI.</param>
    /// <param name="content">Request body.</param>
    /// <param name="configureHeaders">Optional header configuration.</param>
    /// <param name="completionOption">Completion behavior.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The HTTP response.</returns>
    protected Task<HttpResponseMessage> PostAsync((string template, string uri) uri, 
        HttpContent content,
        Action<HttpRequestHeaders>? configureHeaders = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, uri.uri)
        {
            Content = content
        };

        request.Options.Set(TemplateKey, uri.template);
        request.AddHeaderRequestTemplate(uri.template);

        return SendAsync(request, configureHeaders, completionOption, cancellationToken);
    }

    /// <summary>
    /// Executes a POST request to the specified URI.
    /// </summary>
    /// <param name="uri">Target request URI.</param>
    /// <param name="content">Request body.</param>
    /// <param name="configureHeaders">Optional header configuration.</param>
    /// <param name="completionOption">Completion behavior.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The HTTP response.</returns>
    protected Task<HttpResponseMessage> PostAsync(string uri, 
        HttpContent content,
        Action<HttpRequestHeaders>? configureHeaders = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = content
        };

        return SendAsync(request, configureHeaders, completionOption, cancellationToken);
    }
    #endregion

    #region PUT
    /// <summary>
    /// Executes a PUT request using a logical route template for structured logging.
    /// </summary>
    /// <param name="uri">Template and request URI.</param>
    /// <param name="content">Request body.</param>
    /// <param name="configureHeaders">Optional header configuration.</param>
    /// <param name="completionOption">Completion behavior.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The HTTP response.</returns>
    protected Task<HttpResponseMessage> PutAsync((string template, string uri) uri, 
        HttpContent content,
        Action<HttpRequestHeaders>? configureHeaders = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, uri.uri)
        {
            Content = content
        };

        request.Options.Set(TemplateKey, uri.template);
        request.AddHeaderRequestTemplate(uri.template);

        return SendAsync(request, configureHeaders, completionOption, cancellationToken);

    }

    /// <summary>
    /// Executes a PUT request to the specified URI.
    /// </summary>
    /// <param name="uri">Target request URI.</param>
    /// <param name="content">Request body.</param>
    /// <param name="configureHeaders">Optional header configuration.</param>
    /// <param name="completionOption">Completion behavior.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The HTTP response.</returns>
    protected Task<HttpResponseMessage> PutAsync(string uri, 
        HttpContent content,
        Action<HttpRequestHeaders>? configureHeaders = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = content
        };
        return SendAsync(request, configureHeaders, completionOption, cancellationToken);

    }
    #endregion

    #region PATCH
    /// <summary>
    /// Executes a PATCH request using a logical route template for structured logging.
    /// </summary>
    /// <param name="uri">Template and request URI.</param>
    /// <param name="content">Request body.</param>
    /// <param name="configureHeaders">Optional header configuration.</param>
    /// <param name="completionOption">Completion behavior.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The HTTP response.</returns>
    protected Task<HttpResponseMessage> PatchAsync((string template, string uri) uri, 
        HttpContent content,
        Action<HttpRequestHeaders>? configureHeaders = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, uri.uri)
        {
            Content = content
        };

        request.Options.Set(TemplateKey, uri.template);
        request.AddHeaderRequestTemplate(uri.template);

        return SendAsync(request, configureHeaders, completionOption, cancellationToken);

    }

    /// <summary>
    /// Executes a PATCH request to the specified URI.
    /// </summary>
    /// <param name="uri">Target request URI.</param>
    /// <param name="content">Request body.</param>
    /// <param name="configureHeaders">Optional header configuration.</param>
    /// <param name="completionOption">Completion behavior.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The HTTP response.</returns>
    protected Task<HttpResponseMessage> PatchAsync(string uri, 
        HttpContent content,
        Action<HttpRequestHeaders>? configureHeaders = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, uri)
        {
            Content = content
        };

        return SendAsync(request, configureHeaders, completionOption, cancellationToken);
    }
    #endregion

    #region DELETE
    /// <summary>
    /// Executes a DELETE request using a logical route template for structured logging.
    /// </summary>
    /// <param name="uri">Template and request URI.</param>
    /// <param name="configureHeaders">Optional header configuration.</param>
    /// <param name="completionOption">Completion behavior.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The HTTP response.</returns>
    protected Task<HttpResponseMessage> DeleteAsync((string template, string uri) uri,
        Action<HttpRequestHeaders>? configureHeaders = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, uri.uri);

        request.Options.Set(TemplateKey, uri.template);
        request.AddHeaderRequestTemplate(uri.template);

        return SendAsync(request, configureHeaders, completionOption, cancellationToken);

    }

    /// <summary>
    /// Executes a DELETE request to the specified URI.
    /// </summary>
    /// <param name="uri">Target request URI.</param>
    /// <param name="configureHeaders">Optional header configuration.</param>
    /// <param name="completionOption">Completion behavior.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The HTTP response.</returns>
    protected Task<HttpResponseMessage> DeleteAsync(string uri,
        Action<HttpRequestHeaders>? configureHeaders = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, uri);

        return SendAsync(request, configureHeaders, completionOption, cancellationToken);
    }
    #endregion

    /// <summary>
    /// Executes the HTTP request applying default headers, logging
    /// and execution timing consistently across all HTTP verbs.
    /// </summary>
    /// <param name="request">Prepared request message.</param>
    /// <param name="configureHeaders">Optional header configuration.</param>
    /// <param name="completionOption">Completion behavior.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The HTTP response.</returns>
    private async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        Action<HttpRequestHeaders>? configureHeaders = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default)
    {
        request.AddDefaultHeaders();
        request.ConfigureHeaders(configureHeaders);

        await LogRequestAsync(request);

        var stopwatch = Stopwatch.StartNew();
        var response = await _httpClient.SendAsync(request, completionOption, cancellationToken);

        LogResponse(request, response, stopwatch);

        return response;
    }
    #region Validation

    /// <summary>
    /// Validates the HTTP response and propagates API issues as domain notifications.
    /// </summary>
    /// <typeparam name="TResponse">Expected response type.</typeparam>
    /// <param name="response">HTTP response.</param>
    /// <returns>Deserialized response or null.</returns>
    protected virtual async Task<TResponse?> ValidateAndReturn<TResponse>(HttpResponseMessage response)
    {
        await ValidateResponse(response);

        return await response.ReadAsAsync<TResponse>();
    }

    /// <summary>
    /// Processes API errors, generating notifications or throwing exceptions when required.
    /// </summary>
    /// <param name="response">HTTP response.</param>
    /// <param name="throwException">
    /// Indicates whether unexpected status codes should raise exceptions.
    /// </param>
    protected virtual async Task ValidateResponse(HttpResponseMessage response, bool throwException = false)
    {
        if (response.HasErrors(throwException))
        {
            var apiResponse = await response.ReadAsAsync<ApiResponse>();

            if (apiResponse is not null && apiResponse.Issues is not null && apiResponse.Issues.Any())
            {
                foreach (var issue in apiResponse.Issues)
                {
                    if (issue.Details?.Any() != true) continue;

                    foreach (var error in issue.Details)
                        _notification.Notify(new Notification(error.LogLevel ?? LogLevel.None, error.Type, error.Key, error.Value, error.Detail));
                }
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
                    throw new CustomHttpRequestException($"{response.RequestMessage?.Method} - {response.RequestMessage?.RequestUri} - {(int)response.StatusCode} - {response.StatusCode}");
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
    /// Logs the start of the HTTP request including optional headers and payload.
    /// </summary>
    protected virtual async Task LogRequestAsync(HttpRequestMessage request)
    {
        request.Options.TryGetValue(TemplateKey, out var template);

        var headersJson = string.Empty;
        var contentJson = string.Empty;
        if (IsDetailedLoggingEnabled)
        {
            headersJson = $"|Headers:{request.GetHeadersJsonFormat()}";

            if (request?.Content is not null)
                contentJson = $"|Content:{await request.Content.ReadAsStringAsync()}";
        }
        _logger.LogInfo($"Start processing HTTP request {request?.Method.Method} {(string.IsNullOrEmpty(template) ? request?.RequestUri! : _httpClient.BaseAddress + template)}{headersJson}{contentJson}");
    }

    /// <summary>
    /// Logs the end of the HTTP request including duration and status classification.
    /// </summary>
    protected virtual void LogResponse(HttpRequestMessage request, HttpResponseMessage response, Stopwatch stopwatch)
    {
        stopwatch.Stop();

        request.Options.TryGetValue(TemplateKey, out var template);

        string message = $"End processing HTTP request {response?.RequestMessage?.Method} {(string.IsNullOrEmpty(template) ? response?.RequestMessage?.RequestUri : _httpClient.BaseAddress + template)} after {stopwatch.GetFormattedTime()} - {(int)response.StatusCode}-{response.StatusCode}";

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
