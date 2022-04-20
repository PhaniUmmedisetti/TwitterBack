using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace twitter.DTOs;

public record CommentCreateDTO
{

    [JsonPropertyName("user_id")]
    [Required]
    public long UserId { get; set; }

    [JsonPropertyName("post_id")]
    [Required]
    public long PostId { get; set; }

    [JsonPropertyName("text")]
    [Required]
    public string Text { get; set; }

    [JsonPropertyName("created_at")]
    [Required]
    public DateTimeOffset CreatedAt { get; set; }


    [JsonPropertyName("updated_at")]
    [Required]
    public DateTimeOffset UpdatedAt { get; set; }



}

public record CommentUpdateDTO
{
    [JsonPropertyName("user_id")]
    [Required]
    public long UserId { get; set; }


    [JsonPropertyName("post_id")]
    [Required]
    public long PostId { get; set; }

    [JsonPropertyName("text")]
    [Required]
    public string Text { get; set; }
}
