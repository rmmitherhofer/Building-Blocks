using System.Net;

namespace Common.Exceptions;

/// <summary>
/// Represents an exception thrown when a requested resource is not found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Gets or sets the HTTP status code associated with this exception. Default is 404 Not Found.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.NotFound;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NotFoundException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message and a reference to the inner exception that caused this exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception that caused the current exception.</param>
    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
}
