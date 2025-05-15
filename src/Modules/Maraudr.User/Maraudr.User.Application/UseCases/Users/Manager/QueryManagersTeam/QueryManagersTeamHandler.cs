using Application.Security;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.ValueObjects.Users;

namespace Application.UseCases.Users.Manager.QueryManagersTeam;


public class QueryManagersTeamHandler(IUserRepository repository) : IQueryManagersTeamHandler
{
    public async Task<IEnumerable<AbstractUser>> HandleAsync(Guid id,Guid currentUserId)
    {
        var currentUser = await repository.GetByIdAsync(currentUserId);
      
        var user = await repository.GetByIdAsync(id);
      
        if (user == null)
        {
            throw new ArgumentException($"User with ID {id} does not exist.");
        }
        if (user.Role != Role.Manager)
        {
            throw new InvalidOperationException($"User with ID {id} is not a manager.");
        }
        if(!SecurityChecks.CheckIfUsersMatch(id, currentUserId) && !SecurityChecks.CheckIfUserIsAdmin(currentUser)  )
        {
            throw new InvalidOperationException("Internal error : Can't query managers team user");
        }
        var manager = (Maraudr.User.Domain.Entities.Users.Manager)user;
        return manager.Team;
    }
}