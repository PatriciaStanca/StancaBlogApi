namespace StancaBlogApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class CommentCreateDto
{
    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = null!;
}
