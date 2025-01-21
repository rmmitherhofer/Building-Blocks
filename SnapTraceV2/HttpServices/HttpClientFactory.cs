namespace SnapTraceV2.HttpServices;

internal static class HttpClientFactory
{
    private static HttpClient HttpClient = new();

    private static HttpClient IgnoreSslHttpClient = new(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true
    });

    public static HttpClient Create(bool ignoreSslCertificate) => ignoreSslCertificate ? IgnoreSslHttpClient : HttpClient;
}
