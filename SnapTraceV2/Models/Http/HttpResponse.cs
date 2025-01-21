namespace SnapTraceV2.Models.Http;

public class HttpResponse
{
    public int StatusCode { get; private set; }

    public DateTime EndDateTime { get; }

    public ResponseProperties Properties { get; private set; }

    internal HttpResponse(CreateOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(CreateOptions));

        StatusCode = options.StatusCode;
        EndDateTime = options.EndDateTime;
        Properties = options.Properties ?? new(new ResponseProperties.CreateOptions());
    }

    internal void SetStatusCode(int statusCode) => StatusCode = statusCode;

    internal void SetProperties(ResponseProperties properties)
        => Properties = properties ?? throw new ArgumentNullException(nameof(properties));

    internal class CreateOptions
    {
        public int StatusCode { get; set; }
        public DateTime EndDateTime { get; set; }
        public ResponseProperties Properties { get; set; }

        public CreateOptions() => EndDateTime = DateTime.UtcNow;
    }

    internal HttpResponse Clone()
        => new(new CreateOptions
        {
            StatusCode = StatusCode,
            EndDateTime = EndDateTime,
            Properties = Properties.Clone()
        });
}