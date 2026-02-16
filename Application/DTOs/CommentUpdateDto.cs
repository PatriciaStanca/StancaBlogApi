namespace StancaBlogApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class CommentUpdateDto
{
    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}
