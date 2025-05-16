using Maraudr.User.Domain.Entities.Users;

namespace Application.UseCases.Users.User.QueryConnectedUsers;

public interface IQueryConnectedUsersHandler
{
    public Task<List<AbstractUser?>> HanleAsync();
}