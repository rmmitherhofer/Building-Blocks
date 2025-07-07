using System.Text.Json.Serialization;

namespace SnapTrace.Models;

/// <summary>
/// Represents the context information of the current user.
/// </summary>
public class UserContextRequest
{
    /// <summary>
    /// The unique identifier of the user.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? Id { get; set; }

    /// <summary>
    /// The user's full name.
    /// </summary>
    [JsonPropertyName("userName")]
    public string? Name { get; set; }

    /// <summary>
    /// The tenant or user code.
    /// </summary>
    [JsonPropertyName("tenantId")]
    public string? TenantId { get; set; }

    /// <summary>
    /// Indicates whether the user is authenticated.
    /// </summary>
    [JsonPropertyName("isAuthenticated")]
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// The type of authentication used.
    /// </summary>
    [JsonPropertyName("authenticationType")]
    public string? AuthenticationType { get; set; }

    /// <summary>
    /// The roles assigned to the user.
    /// </summary>
    [JsonPropertyName("roles")]
    public IEnumerable<string>? Roles { get; set; }

    /// <summary>
    /// The claims associated with the user.
    /// </summary>
    [JsonPropertyName("claims")]
    public IDictionary<string, string>? Claims { get; set; }
}
