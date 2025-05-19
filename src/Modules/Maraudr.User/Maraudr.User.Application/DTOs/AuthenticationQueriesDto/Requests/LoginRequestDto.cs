using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AuthenticationQueriesDto.Requests;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}