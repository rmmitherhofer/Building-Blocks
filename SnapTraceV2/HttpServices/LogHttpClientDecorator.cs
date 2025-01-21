using SnapTraceV2.Enums;
using SnapTraceV2.Helpers;
using SnapTraceV2.Models;
using System.Diagnostics;
using System.Text;

namespace SnapTraceV2.HttpServices;

internal class LogHttpClientDecorator : IHttpClient
{
    private readonly IHttpClient _decorated;
    public LogHttpClientDecorator(IHttpClient decorated)
    {
        ArgumentNullException.ThrowIfNull(decorated, nameof(IHttpClient));

        _decorated = decorated;
    }

    public ApiResult<T> Post<T>(Uri uri, HttpContent content)
    {
        Stopwatch sw = new();
        sw.Start();

        Log(HttpMethod.Post.Method, uri, content);

        ApiResult<T> result = _decorated.Post<T>(uri, content);

        Log(HttpMethod.Post.Method, uri, result, sw);

        return result;
    }

    public async Task<ApiResult<T>> PostAsync<T>(Uri uri, HttpContent content)
    {
        Stopwatch sw = new();
        sw.Start();

        Log(HttpMethod.Post.Method, uri, content);

        ApiResult<T> result = await _decorated.PostAsync<T>(uri, content);

        Log(HttpMethod.Post.Method, uri, result, sw);

        return result;
    }

    private void Log(string httpMethod, Uri uri, HttpContent content)
    {
        InternalLogHelper.Log($"HTTP \"{httpMethod.ToUpperInvariant()} {uri}\" executing", LogLevel.Debug);

        if (content is not null)
        {
            string contentAsString = content.ReadAsStringAsync().Result;

            StringBuilder sb = new();
            sb.AppendLine($"HTTP \"{httpMethod.ToUpperInvariant()} {uri}\" Content:");
            sb.Append(contentAsString);

            InternalLogHelper.Log(sb.ToString(), LogLevel.Debug);
        }
    }

    private void Log<T>(string httpMethod, Uri uri, ApiResult<T> result, Stopwatch sw)
    {
        sw.Stop();

        InternalLogHelper.Log($"HTTP \"{httpMethod.ToUpperInvariant()} {uri}\" executed. Duration:{sw.ElapsedMilliseconds} StatusCode:{result.StatusCode}", LogLevel.Debug);

        if (result.ResponseContent is not null)
        {
            InternalLogHelper.Log($"HTTP \"{httpMethod.ToUpperInvariant()} {uri}\" Response: {result.ResponseContent}", LogLevel.Debug);
        }
    }
}
