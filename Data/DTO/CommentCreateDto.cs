
namespace StancaBlogApi.Data.DTO;

public class CommentCreateDto
{
    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = null!;
}
