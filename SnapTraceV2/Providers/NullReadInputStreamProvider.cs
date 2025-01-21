using Microsoft.AspNetCore.Http;

namespace SnapTraceV2.Providers;

public class NullReadInputStreamProvider : IReadInputStreamProvider
{
    public string? ReadInputStream(HttpRequest request) => null;
}
