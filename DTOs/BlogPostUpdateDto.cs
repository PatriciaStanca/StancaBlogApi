namespace StancaBlogApi.DTOs;

public class BlogPostUpdateDto
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int CategoryId { get; set; }
}
