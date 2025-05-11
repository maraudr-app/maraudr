using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AuthenticationQueriesDto.Requests;

public class RegisterRequestDto
{
    [Required]
    [MinLength(2)]
    public string Firstname { get; set; } = null!;
    [Required]
    [MinLength(2)]
    public string Lastname { get; set; } = null!;
    
    // Contact Info
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = null!;
    
    // Address
    [Required]
    [MinLength(5)]
    public string Street { get; set; } = null!;
    [Required]
    public string City { get; set; } = null!;
    [Required]
    public string State { get; set; } = null!;
    [Required]
    public string PostalCode { get; set; } = null!;
    [Required]
    public string Country { get; set; } = null!;
    
    
    public List<string> Languages { get; set; } = new();
    
    public Guid? ManagerId { get; set; }
}