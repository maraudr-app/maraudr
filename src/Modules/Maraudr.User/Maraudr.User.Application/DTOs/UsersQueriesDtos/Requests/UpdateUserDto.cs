namespace Application.DTOs.UsersQueriesDtos.Requests;

public class UpdateUserDto
{
    
    public string? Firstname { get; set; } 
    public string? Lastname { get; set; } 
    
    // Contact Info
    public string? Email { get; set; } 
    public string? PhoneNumber { get; set; } 
    
    // Address
    public string? Street { get; set; } 
    public string? City { get; set; } 
    public string? State { get; set; } 
    public string? PostalCode { get; set; } 
    public string? Country { get; set; } 
    
    public string? Biography { get; set; }
    public List<string>? Languages { get; set; } = new();
}