using Application.Security;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Users.User.QueryAllUsers;

public class QueryAllUsersHandler(IUserRepository repository):IQueryAllUsersHandler
{
    public async Task<IEnumerable<AbstractUser>> HandleAsync()
    {
        
      
        return await repository.GetAllAsync();
     
    }

}