namespace StancaBlogApi.Models;

public class Comment
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    // Relations
    public int BlogPostId { get; set; }
    public BlogPost BlogPost { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
