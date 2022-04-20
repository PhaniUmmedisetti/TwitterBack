using System.Text.Json.Serialization;

namespace twitter.Models;

public record Comment
{
    [JsonPropertyName("comment_id")]
    public long CommentId { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

    [JsonPropertyName("post_id")]

    public long PostId { get; set; }

    [JsonPropertyName("created_at")]

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [JsonPropertyName("updated_at")]

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

}