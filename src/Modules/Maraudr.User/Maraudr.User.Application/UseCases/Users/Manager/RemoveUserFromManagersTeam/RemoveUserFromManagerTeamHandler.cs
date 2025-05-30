using Application.Security;
using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.ValueObjects.Users;

namespace Application.UseCases.Users.Manager.RemoveUserFromManagersTeam;

public class RemoveUserFromManagerTeamHandler(IUserRepository repository): IRemoveUserFromManagerTeamHandler
{
    public async Task HandleAsync(Guid managerId, Guid currentUserId, Guid userId)
    {
        var manager = await repository.GetByIdAsync(managerId);
        var user = await repository.GetByIdAsync(userId);
        var connectedUser = await repository.GetByIdAsync(currentUserId);
        
        if(!SecurityChecks.CheckIfUsersMatch(managerId, currentUserId) && !SecurityChecks.CheckIfUserIsAdmin(connectedUser))
        {
            throw new InvalidOperationException("Internal error : Can't delete user");
        }
        
        if (user == null)
        {
            throw new ArgumentException($"User with ID {userId} does not exist.");
        }

        if (manager == null)
        {
            throw new ArgumentException($"User with ID {managerId} does not exist.");
            
        }
        if (manager.Role != Role.Manager)
        {
            throw new InvalidOperationException($"User with ID {managerId} is not a manager.");
        }

        var cManager = (Maraudr.User.Domain.Entities.Users.Manager)manager;
        cManager.RemoveMemberFromTeam(user);
        
        await repository.DeleteAsync(user);

    }
}