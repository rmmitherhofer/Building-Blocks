using System.Net;

namespace Api.Responses;

/// <summary>
/// Represents a standardized structure for returning error details from the API.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets the HTTP status code associated with the error.
    /// </summary>
    public HttpStatusCode StatusCode { get; private set; }

    /// <summary>
    /// Gets the message that describes the error.
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResponse"/> class with the specified status code and error message.
    /// </summary>
    /// <param name="statusCode">The HTTP status code to be returned.</param>
    /// <param name="message">The error message. If null or empty, a default message is used.</param>
    public ErrorResponse(HttpStatusCode statusCode, string message)
    {
        StatusCode = statusCode;

        Message = !string.IsNullOrEmpty(message)
            ? message
            : "An error occurred while processing your request.";
    }
}
