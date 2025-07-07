using System.Net;

namespace Api.Responses;

/// <summary>
/// Represents a standard API response structure used for error, validation, and not found results.
/// </summary>
public class ApiResponse : Response
{
    /// <summary>
    /// Gets the HTTP status code associated with the response.
    /// </summary>
    public HttpStatusCode StatusCode { get; private set; }

    /// <summary>
    /// Gets or sets the correlation identifier used for request tracking.
    /// </summary>
    public string CorrelationId { get; set; }

    /// <summary>
    /// Gets the list of issues returned in the response.
    /// </summary>
    public IEnumerable<IssuerResponse> Issues { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponse"/> class from a not found response.
    /// </summary>
    /// <param name="response">The not found response containing details.</param>
    public ApiResponse(NotFoundResponse response)
    {
        StatusCode = HttpStatusCode.NotFound;
        Issues =
        [
            new(IssuerResponseType.NotFound)
            {
                Title = response.Title,
                Details =  [new(response.Detail)]
            }
        ];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponse"/> class from a validation response.
    /// </summary>
    /// <param name="response">The validation response containing validation errors.</param>
    public ApiResponse(ValidationResponse response)
    {
        StatusCode = HttpStatusCode.BadRequest;
        Issues =
        [
            new(IssuerResponseType.Validation)
            {
                Details = response.Validations,
            }
        ];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponse"/> class from a validation response with a custom status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code to assign to the response.</param>
    /// <param name="response">The validation response containing error details.</param>
    public ApiResponse(HttpStatusCode statusCode, ValidationResponse response)
    {
        StatusCode = statusCode;
        Issues =
        [
            new(IssuerResponseType.Error)
            {
                Title = "An error occurred",
                Details = response.Validations,
            }
        ];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResponse"/> class from a generic error response.
    /// </summary>
    /// <param name="response">The error response containing the message and status code.</param>
    public ApiResponse(ErrorResponse response)
    {
        StatusCode = response.StatusCode;
        Issues =
        [
            new(IssuerResponseType.Error)
            {
                Title = "An error occurred",
                Details = [new(response.Message)]
            }
        ];
    }
}
