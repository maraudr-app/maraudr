using Maraudr.User.Domain.Entities;

namespace Application.UseCases.Manager.QueryManagersTeam;

public interface IQueryManagersTeamHandler
{
    public Task<IEnumerable<AbstractUser>> HandleAsync(Guid id);
}