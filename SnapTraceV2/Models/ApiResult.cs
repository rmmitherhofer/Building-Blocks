namespace SnapTraceV2.Models;

public class ApiResult<T> : ApiResult
{
    public T Result { get; set; }
}

public class ApiResult
{
    public int StatusCode { get; set; }

    public string ResponseContent { get; set; }

    public ApiException Exception { get; set; }

    public bool HasException
    {
        get { return Exception != null; }
    }

    public void EnsureSuccess()
    {
        if (Exception is not null)
        {
            throw new Exception(Exception.ErrorMessage);
        }
    }
}
