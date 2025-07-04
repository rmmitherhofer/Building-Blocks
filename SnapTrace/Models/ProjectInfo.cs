using SnapTrace.Enums;
using System.Text.Json.Serialization;

namespace SnapTrace.Models;

public class ProjectInfo
{
    [JsonPropertyName("Id")]
    public Guid Id { get; set; }

    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Type")]
    public ProjectType Type { get; set; }
}
