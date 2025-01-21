namespace SnapTraceV2;

public class ApiException
{
    public string ErrorMessage { get; set; }

    internal static ApiException Create(string stringResponse)
    {
        ApiException? result = null;

        if (!string.IsNullOrEmpty(stringResponse))
        {
            try
            {
                result = SnapTraceOptionsConfiguration.JsonSerializer.Deserialize<ApiException>(stringResponse);
            }
            catch { }
        }

        if (string.IsNullOrEmpty(result?.ErrorMessage))
        {
            result = new ApiException
            {
                ErrorMessage = "HTTP exception"
            };
        }

        return result;
    }

    internal static ApiException Create(Exception ex)
        => new()
        {
            ErrorMessage = ex.ToString()
        };
}
