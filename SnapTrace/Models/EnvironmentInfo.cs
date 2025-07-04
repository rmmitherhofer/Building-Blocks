using System.Text.Json.Serialization;

namespace SnapTrace.Models;

public class EnvironmentInfo
{
    [JsonPropertyName("MachineName")]
    public string MachineName { get; set; }

    [JsonPropertyName("EnvironmentName")]
    public string EnvironmentName { get; set; }

    [JsonPropertyName("ApplicationVersion")]
    public string ApplicationVersion { get; set; }

    [JsonPropertyName("ThreadId")]
    public int ThreadId { get; set; }
}
