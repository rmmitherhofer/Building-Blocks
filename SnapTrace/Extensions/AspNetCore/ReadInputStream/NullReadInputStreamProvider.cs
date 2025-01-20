using Microsoft.AspNetCore.Http;

namespace SnapTrace.AspNetCore.ReadInputStream
{
    public class NullReadInputStreamProvider : IReadInputStreamProvider
    {
        public string ReadInputStream(HttpRequest request)
        {
            return null;
        }
    }
}
