using SnapTraceV2.Models;

namespace SnapTraceV2.HttpServices;

internal interface IHttpClient
{
    Task<ApiResult<T>> PostAsync<T>(Uri uri, HttpContent content);
    ApiResult<T> Post<T>(Uri uri, HttpContent content);
}
