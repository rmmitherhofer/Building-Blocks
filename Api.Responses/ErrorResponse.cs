using Common.Notifications.Messages;
using System.Net;

namespace Api.Responses;

public class ErrorResponse
{
    public HttpStatusCode StatusCode { get; private set; }
    public string Message { get; private set; }

    public ErrorResponse(HttpStatusCode statusCode, string message)
    {
        StatusCode = statusCode;

        Message = !string.IsNullOrEmpty(message) ? message : "An error occurred while processing your request.";
    }
}
