using System.ComponentModel.DataAnnotations;

namespace Maraudr.Authentication.Application.DTOs.Requests;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}