using Maraudr.User.Domain.Entities.Users;

namespace Application.Security;

public static class SecurityChecks
{
    public static bool CheckIfUserIsAdmin(AbstractUser user)
    {
        ArgumentNullException.ThrowIfNull(user);
        return user.IsUserAdmin();
    }
    
    public static bool CheckIfUsersMatch(Guid id, Guid currentUserId)
    {
        return id == currentUserId;
    }
    
    public static bool CheckIfEmailsMatch(string email, string currentUserEmail)
    {
        return email == currentUserEmail;
    }
}