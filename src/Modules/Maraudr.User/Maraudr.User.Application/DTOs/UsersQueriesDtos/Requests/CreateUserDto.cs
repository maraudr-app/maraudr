namespace Application.DTOs.UsersQueriesDtos.Requests;

public class CreateUserDto
{
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    
    // Contact Info
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    
    // Address
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Country { get; set; } = null!;
    
    public List<string> Languages { get; set; } = new();
    
    public Guid? ManagerId { get; set; }

    public bool IsManager { get; set; } 

}