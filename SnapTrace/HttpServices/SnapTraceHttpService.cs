using Api.Responses;
using Common.Exceptions;
using Common.Http;
using Common.Http.Extensions;
using Common.Json;
using Common.Logs.Extensions;
using Common.Notifications.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnapTrace.Configurations.Settings;
using SnapTrace.Models;
using System.Text;

namespace SnapTrace.HttpServices;

/// <summary>
/// HTTP service responsible for communicating with the SnapTrace logging endpoint.
/// </summary>
public class SnapTraceHttpService : HttpService, ISnapTraceHttpService
{
    private readonly SnapTraceSettings _settings;

    /// <summary>
    /// Initializes a new instance of <see cref="SnapTraceHttpService"/>.
    /// </summary>
    /// <param name="httpClient">The configured HTTP client for sending requests.</param>
    /// <param name="notification">Notification handler for capturing domain notifications.</param>
    /// <param name="logger">Logger instance for internal logging.</param>
    /// <param name="options">SnapTrace configuration settings.</param>
    public SnapTraceHttpService(
        HttpClient httpClient,
        INotificationHandler notification,
        ILogger<SnapTraceHttpService> logger,
        IOptions<SnapTraceSettings> options
    ) : base(httpClient, notification, logger) => _settings = options.Value;

    /// <summary>
    /// Sends the given <see cref="LogContextRequest"/> to the SnapTrace API.
    /// </summary>
    /// <param name="log">The log context to be transmitted.</param>
    public async Task Flush(LogContextRequest log)
    {
        var uri = _settings.Service.EndPoints.Notify;

        var content = JsonExtensions.SerializeContent(log);

        if(_settings.WritePayloadToConsole)
            _logger.LogTrace("Payload: " + JsonExtensions.Serialize(log));

        var response = await PostAsync(uri, content);

        try
        {
            if (response.HasErrors())
                await Print(response);
        }
        catch (CustomHttpRequestException ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    /// <summary>
    /// Reads and prints the details from an HTTP error response returned by the SnapTrace API.
    /// </summary>
    /// <param name="response">The HTTP response containing error details.</param>
    private async Task Print(HttpResponseMessage response)
    {
        StringBuilder sb = new();
        ApiResponse apiResponse = null;

        try
        {
            apiResponse = await response.ReadAsAsync<ApiResponse>();

            if (apiResponse is null)
                throw new CustomHttpRequestException();
        }
        catch (Exception)
        {
            throw new CustomHttpRequestException(
                response.StatusCode,
                $"{response.RequestMessage.Method} - {response.RequestMessage.RequestUri} - {response.StatusCode} - {response.StatusCode}"
            );
        }

        sb.AppendLine($"{nameof(SnapTraceHttpService)}: StatusCode: {response.StatusCode}");

        foreach (var issue in apiResponse.Issues)
        {
            sb.AppendLine($"Type: {issue.DescriptionType}");
            sb.AppendLine($"Title: {issue.Title}");

            foreach (var detail in issue.Details)
                sb.AppendLine($"Level: {detail.LogLevel} - Key: {detail.Key} - Value {detail.Value}");

            switch (issue.Type)
            {
                case IssuerResponseType.NotFound:
                case IssuerResponseType.Validation:
                    _logger.LogWarn(sb.ToString());
                    break;
                case IssuerResponseType.Error:
                    _logger.LogFail(sb.ToString());
                    break;
            }
        }
    }
}
