namespace Maraudr.User.Infrastructure.Security;

public interface IPasswordManager
{ 
      string  HashPassword(string password);
      bool VerifyPassword(string hashedPassword, string password);
}