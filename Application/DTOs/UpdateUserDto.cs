namespace StancaBlogApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class UpdateUserDto
{
    [MinLength(1)]
    [MaxLength(50)]
    public string? Name { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}
