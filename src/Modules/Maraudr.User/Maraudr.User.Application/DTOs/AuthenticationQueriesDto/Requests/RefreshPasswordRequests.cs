using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AuthenticationQueriesDto.Requests;

public class ResetPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}

public class ConfirmResetRequest
{
    [Required]
    public string Token { get; set; }
    
    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; }
}