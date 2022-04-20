using System.Text.Json.Serialization;

namespace twitter.Models;

public record User
{

    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

    [JsonPropertyName("full_name")]
    public string Fullname { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("email")]

    public string Email { get; set; }
}