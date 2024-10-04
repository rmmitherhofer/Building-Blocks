using System.Net;

namespace Common.Exceptions;

public class CustomHttpRequestException : Exception
{
    public HttpStatusCode HttpStatusCode;
    public CustomHttpRequestException() { }
    public CustomHttpRequestException(string message) : base(message) { }
    public CustomHttpRequestException(string message, Exception innerException) : base(message, innerException) { }
    public CustomHttpRequestException(HttpStatusCode httpStatusCode)
    {
        HttpStatusCode = httpStatusCode;
    }
    public CustomHttpRequestException(HttpStatusCode httpStatusCode, string message) : base(message)
    {
        HttpStatusCode = httpStatusCode;
    }
    public CustomHttpRequestException(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(message, innerException)
    {
        HttpStatusCode = httpStatusCode;
    }
}
