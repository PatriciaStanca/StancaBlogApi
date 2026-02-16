namespace StancaBlogApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class BlogPostCreateDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(5000)]
    public string Content { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}
