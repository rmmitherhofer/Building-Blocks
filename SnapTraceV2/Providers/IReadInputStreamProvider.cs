using Microsoft.AspNetCore.Http;

namespace SnapTraceV2.Providers;

internal interface IReadInputStreamProvider
{
    string ReadInputStream(HttpRequest request);
}
