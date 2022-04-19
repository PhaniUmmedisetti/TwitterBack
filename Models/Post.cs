namespace twitter.models;

public record Post
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

}