using System.Text.Json.Serialization;

namespace SocialMedia_Backend.Model.DTO;

public class FieldValidationErrorModel
{
    [JsonPropertyName("default_message")]
    public string DefaultMessage { get; set; }
    [JsonPropertyName("key")]
    public string Key { get; set; }
    [JsonPropertyName("context")]
    public Dictionary<string, object>? Context { get; set; }
}
