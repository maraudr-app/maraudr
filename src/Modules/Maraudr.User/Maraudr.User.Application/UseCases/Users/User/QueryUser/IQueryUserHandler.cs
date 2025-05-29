
using Maraudr.User.Domain.Entities.Users;

namespace Application.UseCases.Users.User.QueryUser;
public interface IQueryUserHandler
{
    public Task<AbstractUser?> HandleAsync(Guid id);
}