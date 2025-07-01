namespace Common.Exceptions;


public class DomainException : Exception
{
    public int StatusCode { get; set; } = 400;
    public DomainException() { }

    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}
