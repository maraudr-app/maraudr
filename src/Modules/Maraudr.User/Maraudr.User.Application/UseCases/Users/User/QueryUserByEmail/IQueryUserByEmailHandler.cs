using Maraudr.User.Domain.Entities.Users;

namespace Application.UseCases.Users.User.QueryUserByEmail;

public interface IQueryUserByEmailHandler
{
    public Task<AbstractUser?> HandleAsync(string email);

}