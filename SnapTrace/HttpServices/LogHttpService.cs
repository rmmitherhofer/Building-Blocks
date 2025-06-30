using Common.Http;
using Common.Notifications.Interfaces;
using Logs.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SnapTrace.Enums;
using SnapTrace.Models;
using System.Net;
using System.Text;

namespace SnapTrace.HttpServices;

public interface ILogHttpService
{
    Task Add(Log log);
}

public class LogHttpService : HttpService, ILogHttpService
{
    private readonly ILogger<LogHttpService> _logger;

    public LogHttpService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, INotificationHandler notification, ILogger<LogHttpService> logger) : base(httpClient, httpContextAccessor, notification, logger)
    {
        _logger = logger;
    }

    public async Task Add(Log log)
    {
        var uri = $"xxxx";

        var itemContet = SerializeContent(log);

        _logger.LogError("Body: " + JsonConvert.SerializeObject(log));

        //var response = await PostAsync(uri, itemContet);

        //if (ResponseHasErrors(response))
        //   _= Print(response, log.Project.Type);
    }

    private async Task Print(HttpResponseMessage response, ProjectType type)
    {
        StringBuilder errorDescription = new();

        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest:
                var validationsResult = new ValidationResult();
                if (type == ProjectType.Webapp)
                {
                    _logger.LogWarn("StatusCode: " + validationsResult.Status.ToString());
                    foreach (var validation in validationsResult.Validations)
                        _logger.LogWarn($"{validation.Timestamp:dd/MM/yyyy HH:mm:ss} - StatusCode: {validationsResult.Status} | Type: {validation.Type} | Message: {validation.Value}");
                }
                else
                {
                    _logger.LogWarn("StatusCode: " + validationsResult.Status.ToString());
                    foreach (var notification in validationsResult.Notifications)
                        _logger.LogWarn($"{notification.Timestamp:dd/MM/yyyy HH:mm:ss} - StatusCode: {validationsResult.Status} | Type: {notification.Type} | Message: {notification.Value}");
                }
                break;

        }

    }
}
