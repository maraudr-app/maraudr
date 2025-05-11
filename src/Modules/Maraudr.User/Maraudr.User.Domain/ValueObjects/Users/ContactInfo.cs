using System.Text.RegularExpressions;

namespace Maraudr.User.Domain.ValueObjects.Users;

public class ContactInfo 
{
    public string Email { get; } = null!;
    private string PhoneNumber { get; } = null!;

    public ContactInfo(string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        
        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));
        
        if (!IsValidPhoneNumber(phoneNumber))
            throw new ArgumentException("Invalid phone number format", nameof(phoneNumber));

        Email = email;
        PhoneNumber = phoneNumber;
    }

    // Required by EF Core
    private ContactInfo() { }

    private static bool IsValidEmail(string email)
    {
        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());
        return digitsOnly.Length >= 7 && digitsOnly.Length <= 15;
    }

    public string GetEmail()
    {
        return Email;
    }
    
    public string GetPhoneNumber()
    {
        return PhoneNumber;
    }
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (GetType() != obj.GetType()) return false;

        var other = (ContactInfo)obj;
        return Email == other.Email &&
               PhoneNumber == other.PhoneNumber;
    }

    // Implémentation de GetHashCode
    public override int GetHashCode()
    {
        return HashCode.Combine(Email, PhoneNumber);
    }

}