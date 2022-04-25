using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace twitter.DTOs;

public record UserLoginDTO
{
    [Required]
    [JsonPropertyName("email")]
    [MinLength(3)]
    [MaxLength(255)]
    public string Email { get; set; }

    [Required]
    [JsonPropertyName("password")]
    [MaxLength(255)]
    public string Password { get; set; }
}

public record UserLoginResDTO
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

    [JsonPropertyName("full_name")]
    public string FullName { get; set; }
}

public record UserCreateDto
{


    [JsonPropertyName("full_name")]
    [Required]
    [MaxLength(50)]
    public string Fullname { get; set; }


    [JsonPropertyName("password")]
    [Required]
    public string Password { get; set; }

    [JsonPropertyName("email")]
    [MaxLength(255)]
    public string Email { get; set; }

}

public record UserUpdateDto
{


    [JsonPropertyName("full_name")]

    public string Fullname { get; set; }



}


