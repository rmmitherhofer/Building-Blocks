using System.Text;

namespace SnapTraceV2.Factories;

internal static class ReadStreamStrategyFactory
{
    public static IReadStreamStrategy Create(Stream stream, Encoding encoding, string contentType)
    {
        ArgumentNullException.ThrowIfNull(stream, nameof(Stream));

        ArgumentNullException.ThrowIfNull(encoding, nameof(Encoding));

        if (!stream.CanRead)
            return new NullReadStream();

        if (ShouldUseReadStreamAsString(stream.Length, contentType))
            return new ReadStreamAsString(stream, encoding);

        return new ReadStreamToTemporaryFile(stream);
    }

    private static bool ShouldUseReadStreamAsString(long contentLength, string contentType)
    {
        if (string.IsNullOrEmpty(contentType)) return false;

        string[] allowedContentTypes = ["application/json", "application/xml", "text/plain", "text/xml"];

        contentType = contentType.ToLowerInvariant();

        bool match = allowedContentTypes.Any(p => contentType.Contains(p));

        if (!match) return false;

        return contentLength <= Constants.ReadStreamAsStringMaxContentLengthInBytes;
    }
}
