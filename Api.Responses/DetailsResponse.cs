using System.Net;

namespace Api.Responses;

public class DetailsResponse : Response
{
    public HttpStatusCode StatusCode { get; private set; }
    public string CorrelationId { get; set; }
    public IEnumerable<IssuerResponse> Issues { get; private set; }

    public DetailsResponse(NotFoundResponse response)
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

    public DetailsResponse(ValidationResponse response)
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

    public DetailsResponse(HttpStatusCode statusCode, ValidationResponse response)
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

    public DetailsResponse(ErrorResponse response)
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
