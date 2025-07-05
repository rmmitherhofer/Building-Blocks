using SnapTrace.Enums;
using System.Text.Json.Serialization;

namespace SnapTrace.Models;


/// <summary>
/// Contains basic information about a project.
/// </summary>
public class ProjectInfo
{
    /// <summary>
    /// Unique identifier of the project.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the project.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Type of the project.
    /// </summary>
    [JsonPropertyName("type")]
    public ProjectType Type { get; set; }
}


