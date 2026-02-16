namespace StancaBlogApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class UpdateUserDto
{
    [MaxLength(50)]
    public string? Name { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}
