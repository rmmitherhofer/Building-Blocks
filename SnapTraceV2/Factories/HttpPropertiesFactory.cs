using SnapTraceV2.Helpers;
using SnapTraceV2.Models.Http;

namespace SnapTraceV2.Factories;

internal static class HttpPropertiesFactory
{
    public static HttpProperties Create(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

        return new(new HttpRequest(new HttpRequest.CreateOptions
        {
            HttpMethod = HttpMethod.Get.ToString(),
            Url = InternalHelper.GenerateUri(url),
            MachineName = InternalHelper.GetMachineName()
        }));
    }
}
