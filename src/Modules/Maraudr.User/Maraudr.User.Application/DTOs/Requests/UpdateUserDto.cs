namespace Maraudr.User.Application.DTOs.Requests;

public class UpdateUserDto
{
    public string? Firstname { get; set; } = null!;
    public string? Lastname { get; set; } = null!;
    public string? Street { get; set; } = null!;
    public string? City { get; set; } = null!;
    public string? State { get; set; } = null!;
    public string? PostalCode { get; set; } = null!;
    public string? Country { get; set; } = null!;
    public string? Email { get; set; } = null!;
    public string? PhoneNumber { get; set; } = null!;
    public string? AccountType { get; set; } = null!;
}