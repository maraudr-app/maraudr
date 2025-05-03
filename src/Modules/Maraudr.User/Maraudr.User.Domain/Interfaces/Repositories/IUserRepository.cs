

namespace Maraudr.User.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<Maraudr.Domain.Entities.AbstractUser?> GetByIdAsync(Guid id);
    Task<IEnumerable<Maraudr.Domain.Entities.AbstractUser?>> GetAllAsync();
    Task AddAsync(Maraudr.Domain.Entities.AbstractUser user);
    //Task<User?> UpdateAsync(User user);
    Task DeleteAsync(Maraudr.Domain.Entities.AbstractUser user);
}