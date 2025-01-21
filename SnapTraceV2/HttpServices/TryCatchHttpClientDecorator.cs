using SnapTraceV2.Helpers;
using SnapTraceV2.Models;

namespace SnapTraceV2.HttpServices;

internal class TryCatchHttpClientDecorator : IHttpClient
{
    private readonly IHttpClient _decorated;
    public TryCatchHttpClientDecorator(IHttpClient decorated)
    {
        ArgumentNullException.ThrowIfNull(decorated, nameof(IHttpClient));

        _decorated = decorated;
    }

    public async Task<ApiResult<T>> PostAsync<T>(Uri uri, HttpContent content)
    {
        ApiResult<T> result;

        try
        {
            result = await _decorated.PostAsync<T>(uri, content).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            InternalLogHelper.LogException(ex);

            result = new ApiResult<T>
            {
                Exception = ApiException.Create(ex)
            };
        }

        return result;
    }

    public ApiResult<T> Post<T>(Uri uri, HttpContent content)
    {
        ApiResult<T> result;

        try
        {
            result = _decorated.Post<T>(uri, content);
        }
        catch (Exception ex)
        {
            InternalLogHelper.LogException(ex);

            result = new ApiResult<T>
            {
                Exception = ApiException.Create(ex)
            };
        }

        return result;
    }
}
