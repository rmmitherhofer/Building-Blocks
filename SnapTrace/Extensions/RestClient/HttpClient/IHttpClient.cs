namespace SnapTrace.RestClient.HttpClient
{
    internal interface IHttpClient
    {
        Task<ApiResult<T>> PostAsync<T>(Uri uri, HttpContent content);
        ApiResult<T> Post<T>(Uri uri, HttpContent content);
    }
}
