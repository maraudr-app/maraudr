

using Maraudr.User.Domain.Entities;

namespace Maraudr.User.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<AbstractUser?> GetByIdAsync(Guid id);
    Task<IEnumerable<AbstractUser?>> GetAllAsync();
    Task AddAsync(AbstractUser user);
    //Task<User?> UpdateAsync(User user);
    Task DeleteAsync(AbstractUser user);

    Task UpdateAsync(AbstractUser user);
}