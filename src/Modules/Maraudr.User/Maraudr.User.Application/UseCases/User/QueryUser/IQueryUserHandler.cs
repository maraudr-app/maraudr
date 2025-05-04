
using Maraudr.User.Domain.Entities;

namespace Application.UseCases.User.QueryUser;
public interface IQueryUserHandler
{
    public Task<AbstractUser?> HandleAsync(Guid id);
}