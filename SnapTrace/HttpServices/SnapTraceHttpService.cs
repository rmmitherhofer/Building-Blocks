using Api.Responses;
using Common.Exceptions;
using Common.Extensions;
using Common.Http;
using Common.Notifications.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SnapTrace.Configurations.Settings;
using SnapTrace.Models;
using System.Net;
using System.Text;

namespace SnapTrace.HttpServices;

public interface ISnapTraceHttpService
{
    Task Add(Log log);
}

public class SnapTraceHttpService : HttpService, ISnapTraceHttpService
{
    private readonly SnapTraceSettings _settings;
    public SnapTraceHttpService(HttpClient httpClient,
        IHttpContextAccessor accessor,
        INotificationHandler notification,
        ILogger<SnapTraceHttpService> logger,
        IOptions<SnapTraceSettings> options) : base(httpClient, accessor, notification, logger)
    {
        _settings = options.Value;
    }

    public async Task Add(Log log)
    {
        var uri = _settings.Service.EndPoints.Notify;

        var content = SerializeContent(log);

        _logger.LogTrace("Body: " + JsonConvert.SerializeObject(log));

        var response = await PostAsync(uri, content);

        try
        {
            if (ResponseHasErrors(response))
                await Print(response);
        }
        catch (CustomHttpRequestException ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    private async Task Print(HttpResponseMessage response)
    {
        StringBuilder sb = new();
        DetailsResponse details = null;

        try
        {
            details = await DeserializeObjectResponseAsync<DetailsResponse>(response);
        }
        catch (Exception)
        {
            throw new CustomHttpRequestException(response.StatusCode, $"{response.RequestMessage.Method} - {response.RequestMessage.RequestUri} - {response.StatusCode} - {response.StatusCode}");
        }

        sb.AppendLine($"{nameof(SnapTraceHttpService)}: Status Code: {response.StatusCode}");

        foreach (var issue in details.Issues)
        {
            sb.AppendLine($"Type: {issue.DescriptionType}");
            sb.AppendLine($"Title: {issue.Title}");

            foreach (var detail in issue.Details)
                sb.AppendLine($"Level: {detail.LogLevel.ToString()} - Key: {detail.Key} - Value {detail.Value}");

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
