namespace SnapTraceV2.Models.Http;

public class ResponseProperties
{
    public IEnumerable<KeyValuePair<string, string?>> Headers { get; }
    public long ContentLength { get; }

    internal ResponseProperties(CreateOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(CreateOptions));

        if (options.ContentLength < 0) throw new ArgumentException(nameof(options.ContentLength));

        Headers = options.Headers?.Where(p => !string.IsNullOrWhiteSpace(p.Key)).ToList() ?? [];

        ContentLength = options.ContentLength;
    }

    internal class CreateOptions
    {
        public IEnumerable<KeyValuePair<string, string?>> Headers { get; set; }
        public long ContentLength { get; set; }
    }

    internal ResponseProperties Clone()
        => new(new CreateOptions
        {
            Headers = Headers,
            ContentLength = ContentLength
        });
}
