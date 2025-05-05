using Maraudr.User.Domain.Entities;
using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.ValueObjects;

namespace Application.UseCases.Manager.QueryManagersTeam;


public class QueryManagersTeamHandler(IUserRepository repository) : IQueryManagersTeamHandler
{
    public async Task<IEnumerable<AbstractUser>> HandleAsync(Guid id)
    {
        var user = await repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {id} does not exist.");
        }
        if (user.Role != Role.Manager)
        {
            throw new InvalidOperationException($"User with ID {id} is not a manager.");
        }
        var manager = (Maraudr.User.Domain.Entities.Manager)user;
        return manager.Team;
        
    }
}