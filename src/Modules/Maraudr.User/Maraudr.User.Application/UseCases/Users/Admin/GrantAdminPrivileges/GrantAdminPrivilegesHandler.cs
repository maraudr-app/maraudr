using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Users.Admin.GrantAdminPrivileges;

public class GrantAdminPrivilegesHandler(IUserRepository repository):IGrantAdminPrivilegesHandler
{
    public async Task HandleAsync(Guid targetUserId)
    {
        // TODO: Nécessite authentification !
        
        // Récupérer l'utilisateur courant (celui qui fait la demande)
        /* var currentUserId = _currentUserService.GetUserId();
        if (currentUserId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var currentUser = await repository.GetByIdAsync(currentUserId.Value);
        if (currentUser == null)
            throw new UnauthorizedAccessException("Current user not found");

        if (!currentUser.IsUserAdmin())
            throw new UnauthorizedAccessException("Only admins can grant admin privileges");

        var targetUser = await repository.GetByIdAsync(targetUserId);
        if (targetUser == null)
            throw new InvalidOperationException("Target user not found");

        targetUser.GrantAdminPrivileges(currentUser);

        await repository.UpdateAsync(targetUser);*/
    }
}