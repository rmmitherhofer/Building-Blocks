using System.Net;

namespace Common.Exceptions;

/// <summary>
/// Represents errors that occur within the domain logic of the application.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Gets or sets the HTTP status code associated with the domain exception. Defaults to 400 Bad Request.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class.
    /// </summary>
    public DomainException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public DomainException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}
