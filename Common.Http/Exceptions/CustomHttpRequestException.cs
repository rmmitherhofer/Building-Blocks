﻿using System.Net;

namespace Common.Http.Exceptions;

/// <summary>
/// Represents errors that occur during HTTP requests with an associated HTTP status code.
/// </summary>
public class CustomHttpRequestException : Exception
{
    /// <summary>
    /// Gets the HTTP status code associated with the exception.
    /// </summary>
    public HttpStatusCode HttpStatusCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomHttpRequestException"/> class.
    /// </summary>
    public CustomHttpRequestException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomHttpRequestException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CustomHttpRequestException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomHttpRequestException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public CustomHttpRequestException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomHttpRequestException"/> class with a specified HTTP status code.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code that caused the error.</param>
    public CustomHttpRequestException(HttpStatusCode httpStatusCode)
    {
        HttpStatusCode = httpStatusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomHttpRequestException"/> class with a specified HTTP status code and error message.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code that caused the error.</param>
    /// <param name="message">The error message.</param>
    public CustomHttpRequestException(HttpStatusCode httpStatusCode, string message) : base(message)
    {
        HttpStatusCode = httpStatusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomHttpRequestException"/> class with a specified HTTP status code, error message, and a reference to the inner exception.
    /// </summary>
    /// <param name="httpStatusCode">The HTTP status code that caused the error.</param>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception that is the cause of this exception.</param>
    public CustomHttpRequestException(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(message, innerException)
    {
        HttpStatusCode = httpStatusCode;
    }
}
