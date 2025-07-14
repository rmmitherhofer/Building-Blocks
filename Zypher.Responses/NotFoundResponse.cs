using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json.Serialization;

namespace Zypher.Responses;

/// <summary>
/// Represents a standard 404 Not Found API response with optional detail information.
/// </summary>
public class NotFoundResponse : NotFoundResult
{
    /// <summary>
    /// Gets the HTTP status code representing the response.
    /// </summary>
    [JsonPropertyName("status")]
    public HttpStatusCode Status { get; set; }

    /// <summary>
    /// Gets the title that summarizes the type of response.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// Gets the detailed message associated with the response.
    /// </summary>
    [JsonPropertyName("detail")]
    public string Detail { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundResponse"/> class.
    /// </summary>
    public NotFoundResponse() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundResponse"/> class with an optional detail message.
    /// </summary>
    /// <param name="detail">An optional detailed message to provide context about the not found result.</param>
    public NotFoundResponse(string? detail = null)
    {
        Status = HttpStatusCode.NotFound;
        Title = "Not found";
        Detail = detail ?? "Query returned no results.";
    }
}
