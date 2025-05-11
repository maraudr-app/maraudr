using Maraudr.User.Domain.Entities.Users;

namespace Application.UseCases.Users.Manager.QueryManagersTeam;

public interface IQueryManagersTeamHandler
{
    public Task<IEnumerable<AbstractUser>> HandleAsync(Guid id);
}