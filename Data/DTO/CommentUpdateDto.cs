
namespace StancaBlogApi.Data.DTO;

public class CommentUpdateDto
{
    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}
