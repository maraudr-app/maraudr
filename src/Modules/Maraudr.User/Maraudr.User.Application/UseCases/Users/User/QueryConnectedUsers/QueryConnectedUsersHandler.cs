using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Users.User.QueryConnectedUsers;

public class QueryConnectedUsersHandler(IUserRepository repository):IQueryConnectedUsersHandler
{
    public async Task<List<AbstractUser?>> HanleAsync()
    {
        List<AbstractUser?> connectedUSers = [];
        var users = await repository.GetAllAsync();

        foreach (var user in users)
        {
            if(user.IsActive) connectedUSers.Add(user);
        }
        return connectedUSers;
    }
}