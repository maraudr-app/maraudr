
namespace Maraudr.User.Infrastructure.Security;

public class PasswordManager : IPasswordManager
{
    public  string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public  bool VerifyPassword(string hashedPassword, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}