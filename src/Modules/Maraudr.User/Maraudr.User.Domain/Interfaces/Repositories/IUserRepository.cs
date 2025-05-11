

using Maraudr.User.Domain.Entities.Users;

namespace Maraudr.User.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<AbstractUser?> GetByIdAsync(Guid id);
    Task<IEnumerable<AbstractUser>> GetAllAsync();
    Task AddAsync(AbstractUser user);
    //Task<User?> UpdateAsync(User user);
    Task DeleteAsync(AbstractUser user);

    Task UpdateAsync(AbstractUser user);
    Task<IEnumerable<AbstractUser>> SearchByNameAsync(string searchTerm);
    Task<AbstractUser?> GetByEmailAsync(string email);

}