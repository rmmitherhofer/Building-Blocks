using SnapTrace.RestClient.Models;
using SnapTrace.RestClient.Requests.CreateRequestLog;

namespace SnapTrace.RestClient.Api
{
    public interface IPublicApi
    {
        Task<ApiResult<RequestLog>> CreateRequestLogAsync(CreateRequestLogRequest request, IEnumerable<Requests.CreateRequestLog.File> files = null);
        ApiResult<RequestLog> CreateRequestLog(CreateRequestLogRequest request, IEnumerable<Requests.CreateRequestLog.File> files = null);
    }
}
