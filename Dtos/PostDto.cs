using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace twitter.DTOs;

public record PostCreateDTO
{
    [JsonPropertyName("title")]
    [Required]
    [MinLength(3)]
    [MaxLength(255)]
    public string Title { get; set; }


    [JsonPropertyName("user_id")]
    [Required]
    public long UserId { get; set; }



    [JsonPropertyName("created_at")]
    [Required]
    public DateTimeOffset CreatedAt { get; set; }


    [JsonPropertyName("updated_at")]
    [Required]
    public DateTimeOffset UpdatedAt { get; set; }


}

public record PostUpdateDTO
{

    [JsonPropertyName("title")]
    [MinLength(3)]
    [MaxLength(255)]
    public string Title { get; set; } = null;

}
