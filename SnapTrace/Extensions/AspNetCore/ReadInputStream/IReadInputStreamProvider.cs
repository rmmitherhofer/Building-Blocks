using Microsoft.AspNetCore.Http;

namespace SnapTrace.AspNetCore.ReadInputStream
{
    internal interface IReadInputStreamProvider
    {
        string ReadInputStream(HttpRequest request);
    }
}
