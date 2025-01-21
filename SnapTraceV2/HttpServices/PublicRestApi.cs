using SnapTraceV2.Helpers;
using SnapTraceV2.Models;
using SnapTraceV2.Models.Requests;
using SnapTraceV2.Models.Responses;
using System.Text;

namespace SnapTraceV2.HttpServices;

public class PublicRestApi : IPublicApi
{
    private const string ENDPOINT = "api/public/v1.0/createRequestLog";
    private readonly IHttpClient _httpClient;
    private readonly string _baseUrl;
    public PublicRestApi(string baseUrl, bool ignoreSslCertificate = false)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentNullException(nameof(baseUrl));

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out _))
            throw new ArgumentException($"{nameof(baseUrl)} is not a valid URL");

        _baseUrl = baseUrl;

        _httpClient = new LogHttpClientDecorator(
            new TryCatchHttpClientDecorator(new DefaultHttpClient(ignoreSslCertificate))
        );
    }

    public ApiResult<RequestLogResponse> CreateRequestLog(CreateRequestLogRequest request, IEnumerable<FileRequest>? files = null)
    {
        Uri url = InternalHelper.BuildUri(_baseUrl, ENDPOINT);
        MultipartFormDataContent content = CreateMultipartFormDataContent(request, files);

        return _httpClient.Post<RequestLogResponse>(url, content);
    }

    public async Task<ApiResult<RequestLogResponse>> CreateRequestLogAsync(CreateRequestLogRequest request, IEnumerable<FileRequest>? files = null)
    {
        Uri url = InternalHelper.BuildUri(_baseUrl, ENDPOINT);
        MultipartFormDataContent content = CreateMultipartFormDataContent(request, files);

        return await _httpClient.PostAsync<RequestLogResponse>(url, content).ConfigureAwait(false);
    }

    private MultipartFormDataContent CreateMultipartFormDataContent(CreateRequestLogRequest request, IEnumerable<FileRequest>? files = null)
    {
        ArgumentNullException.ThrowIfNull(request);

        MultipartFormDataContent form = [];

        if (request is not null)
        {
            string json = SnapTraceOptionsConfiguration.JsonSerializer.Serialize(request);

            form.Add(new StringContent(json, Encoding.UTF8, "application/json"), "RequestLog");
        }

        if (files is not null)
        {
            foreach (var file in files)
            {
                if (!File.Exists(file.FilePath)) continue;

                form.Add(new ByteArrayContent(File.ReadAllBytes(file.FilePath)), "Files", file.FileName);
            }
        }

        return form;
    }
}
