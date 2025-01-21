namespace SnapTraceV2.Models.Http;

public class RequestProperties
{
    public IEnumerable<KeyValuePair<string, string?>> Headers { get; }
    public IEnumerable<KeyValuePair<string, string?>> Cookies { get; }
    public IEnumerable<KeyValuePair<string, string?>> QueryString { get; }
    public IEnumerable<KeyValuePair<string, string?>> FormData { get; }
    public IEnumerable<KeyValuePair<string, string?>> ServerVariables { get; }
    public IEnumerable<KeyValuePair<string, string>> Claims { get; private set; }
    public string InputStream { get; }

    internal RequestProperties(CreateOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(CreateOptions));

        Headers = options.Headers?.Where(p => !string.IsNullOrWhiteSpace(p.Key)).ToList() ?? [];
        Cookies = options.Cookies?.Where(p => !string.IsNullOrWhiteSpace(p.Key)).ToList() ?? [];
        QueryString = options.QueryString?.Where(p => !string.IsNullOrWhiteSpace(p.Key)).ToList() ?? [];
        FormData = options.FormData?.Where(p => !string.IsNullOrWhiteSpace(p.Key)).ToList() ?? [];
        ServerVariables = options.ServerVariables?.Where(p => !string.IsNullOrWhiteSpace(p.Key)).ToList() ?? [];
        Claims = options.Claims?.Where(p => !string.IsNullOrWhiteSpace(p.Key)).ToList() ?? [];
        InputStream = options.InputStream;
    }

    internal void SetClaims(IEnumerable<KeyValuePair<string, string>> claims)
    {
        if (claims is null) return;

        Claims = claims.ToList();
    }

    internal class CreateOptions
    {
        public IEnumerable<KeyValuePair<string, string?>> Headers { get; set; }
        public IEnumerable<KeyValuePair<string, string?>> Cookies { get; set; }
        public IEnumerable<KeyValuePair<string, string?>> QueryString { get; set; }
        public IEnumerable<KeyValuePair<string, string?>> FormData { get; set; }
        public IEnumerable<KeyValuePair<string, string?>> ServerVariables { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Claims { get; set; }
        public string? InputStream { get; set; }
    }

    internal RequestProperties Clone()
        => new(new CreateOptions
        {
            Headers = Headers,
            Cookies = Cookies,
            QueryString = QueryString,
            FormData = FormData,
            ServerVariables = ServerVariables,
            Claims = Claims,
            InputStream = InputStream
        });
}