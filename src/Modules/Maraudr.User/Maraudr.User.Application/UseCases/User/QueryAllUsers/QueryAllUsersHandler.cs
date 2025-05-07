using Maraudr.User.Domain.Entities;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.User.QueryAllUsers;

public class QueryAllUsersHandler(IUserRepository repository):IQueryAllUsersHandler
{
    public async Task<IEnumerable<AbstractUser>> HandleAsync()
    {
        return await repository.GetAllAsync();
     
    }

}