using Maraudr.User.Domain.Entities;

namespace Application.UseCases.User.QueryUserByEmail;

public interface IQueryUserByEmailHandler
{
    public Task<AbstractUser?> HandleAsync(string email);

}