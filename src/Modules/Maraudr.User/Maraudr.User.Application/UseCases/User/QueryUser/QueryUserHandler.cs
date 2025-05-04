using Application.UseCases.User.QueryUser;
using Maraudr.User.Domain.Entities;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.User.QueryUser;

public class QueryUserHandler(IUserRepository repository):IQueryUserHandler
{
    public async Task<AbstractUser?> HandleAsync(Guid id)
    {
        return await repository.GetByIdAsync(id);
    }
}