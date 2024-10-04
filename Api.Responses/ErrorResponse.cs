using Common.Notifications.Messages;
using System.Net;

namespace Api.Responses;

public class ErrorResponse
{
    public HttpStatusCode? StatusCode { get; private set; }
    /// <summary>
    /// Notificações
    /// </summary>
    public IEnumerable<Notification> Errors { get; private set; }

    public ErrorResponse(HttpStatusCode? statusCode, IEnumerable<Notification> notifications)
    {
        StatusCode = statusCode;

        Errors = notifications;
    }

    public ErrorResponse(IEnumerable<Notification> notifications)
    {
        StatusCode = HttpStatusCode.InternalServerError;

        Errors = notifications;
    }
}
