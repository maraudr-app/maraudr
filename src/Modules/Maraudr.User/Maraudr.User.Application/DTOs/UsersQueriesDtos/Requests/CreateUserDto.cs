using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UsersQueriesDtos.Requests;

public class CreateUserDto
{
    [Required]
    [MinLength(2)]
    public string Firstname { get; set; } = null!;
    [Required]
    [MinLength(2)]
    public string Lastname { get; set; } = null!;
    [Required]
    [EmailAddress]
    // Contact Info
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

    public bool IsManager { get; set; } 
    
    [Required]
    [PasswordPropertyText]
    [MinLength(8, ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Le mot de passe doit contenir au moins une lettre majuscule, une lettre minuscule et un chiffre.")]
    public required string Password { get; set; }

}