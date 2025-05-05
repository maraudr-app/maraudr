using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.ValueObjects;

namespace  Application.UseCases.Manager.AddUserToManagersTeam;

public class AddUserToManagersTeamHandler(IUserRepository repository):IAddUserToManagersTeamHandler
{
    public async Task HandleAsync(Guid id, Guid userId)
    {
        
        //TODO : factoriuser verif en fonction qui retourne couple(manager,user)
        
        
        var manager = await repository.GetByIdAsync(id);
        var user = await repository.GetByIdAsync(userId);
        
        if (user == null)
        {
            throw new ArgumentException($"User with ID {userId} does not exist.");
        }

        if (manager == null)
        {
            throw new ArgumentException($"User with ID {id} does not exist.");
            
        }
        if (manager.Role != Role.Manager)
        {
            throw new InvalidOperationException($"User with ID {id} is not a manager.");
        }

        var cManager = (Maraudr.User.Domain.Entities.Manager)manager;
        
        cManager.AddMemberToTeam(user);
        await repository.UpdateAsync(manager);
       
    }
}