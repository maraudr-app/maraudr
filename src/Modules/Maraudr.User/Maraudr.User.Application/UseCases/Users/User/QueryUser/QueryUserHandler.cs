using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Users.User.QueryUser;

public class QueryUserHandler(IUserRepository repository):IQueryUserHandler
{
    public async Task<AbstractUser?> HandleAsync(Guid id)
    {
        return await repository.GetByIdAsync(id);
    }
}