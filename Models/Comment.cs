namespace twitter.models;

public record omment
{
    public int CommentId { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Text { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}