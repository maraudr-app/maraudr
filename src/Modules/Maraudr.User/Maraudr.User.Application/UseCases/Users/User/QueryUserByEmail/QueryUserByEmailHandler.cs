using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Users.User.QueryUserByEmail;

public class QueryUserByEmailHandler(IUserRepository repository): IQueryUserByEmailHandler
{
    public async Task<AbstractUser?> HandleAsync(string email)
    {
        return await repository.GetByEmailAsync(email);
    }
}