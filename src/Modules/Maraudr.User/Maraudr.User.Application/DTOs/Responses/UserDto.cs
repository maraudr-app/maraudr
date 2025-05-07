using System;
using Maraudr.Application.DTOs;

namespace Maraudr.User.Application.DTOs.Responses;

public class UserDto
{
    public Guid Id { get; set; }
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoggedIn { get; set; }
    public bool IsActive { get; set; }
    
    public AddressDto Address { get; set; } = null!;
    public ContactInfoDto ContactInfo { get; set; } = null!;
    public string AccountType { get; set; } = null!;
}



