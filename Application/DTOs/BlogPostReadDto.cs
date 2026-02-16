namespace StancaBlogApi.DTOs;

public class BlogPostReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";

    public int UserId { get; set; }
    public string UserName { get; set; } = "";

    public List<CommentReadDto> Comments { get; set; } = new();
}
