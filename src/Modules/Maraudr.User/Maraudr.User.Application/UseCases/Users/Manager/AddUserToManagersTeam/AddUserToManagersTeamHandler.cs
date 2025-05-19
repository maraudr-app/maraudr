using Application.Security;
using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.ValueObjects.Users;

namespace  Application.UseCases.Users.Manager.AddUserToManagersTeam;

public class AddUserToManagersTeamHandler(IUserRepository repository):IAddUserToManagersTeamHandler
{
    public async Task HandleAsync(Guid managerId, Guid currentUserId, Guid userId)
    {
        
        //TODO : factoriuser verif en fonction qui retourne couple(manager,user)
        //todo: changer admin verif
        var currentUser = await repository.GetByIdAsync(currentUserId);
        
        if(!SecurityChecks.CheckIfUsersMatch(managerId, currentUserId) && !SecurityChecks.CheckIfUserIsAdmin(currentUser))
        {
            throw new InvalidOperationException("Internal error : Can't add user");
        }
        var manager = await repository.GetByIdAsync(managerId);
        var user = await repository.GetByIdAsync(userId);
        
        if (user == null)
        {
            throw new ArgumentException($"User with ID {userId} does not exist.");
        }

        if (manager == null)
        {
            throw new ArgumentException($"User with ID {managerId} does not exist.");
            
        }
        if (manager.Role != Role.Manager || manager.IsUserAdmin())
        {
            throw new InvalidOperationException($"User with ID {managerId} doesn't have the rights to add a user to his team.");
        }

        var cManager = (Maraudr.User.Domain.Entities.Users.Manager)manager;
        
        cManager.AddMemberToTeam(user);
        await repository.UpdateAsync(cManager);
       
    }
}