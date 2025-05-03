

using Maraudr.Domain.Entities;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Maraudr.User.Infrastructure.Repositories
{
    public class UserRepository(UserContext context) : IUserRepository
    {
        private readonly UserContext _context = context;
        public async Task<Maraudr.Domain.Entities.AbstractUser?> GetByIdAsync(Guid id)
        {
            return await _context.GetUserByIdAsync(id);
        }

        public async Task<IEnumerable<Maraudr.Domain.Entities.AbstractUser?>> GetAllAsync()
        {
            return await _context.GetAllUsersAsync();
        }

        public async Task AddAsync(Maraudr.Domain.Entities.AbstractUser user)
        {
            await _context.AddUserAsync(user) ;
        }
        
        public async Task DeleteAsync(Maraudr.Domain.Entities.AbstractUser user)
        {
            await _context.DeleteUserAsync(user.Id);
        }
    }
}
