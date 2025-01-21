using SnapTraceV2.Models;
using SnapTraceV2.Models.Requests;
using SnapTraceV2.Models.Responses;

namespace SnapTraceV2.HttpServices;

public interface IPublicApi
{
    Task<ApiResult<RequestLogResponse>> CreateRequestLogAsync(CreateRequestLogRequest request, IEnumerable<FileRequest>? files = null);
    ApiResult<RequestLogResponse> CreateRequestLog(CreateRequestLogRequest request, IEnumerable<FileRequest>? files = null);
}
