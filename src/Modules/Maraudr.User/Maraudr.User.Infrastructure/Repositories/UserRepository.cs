

using Maraudr.Domain;
using Maraudr.User.Domain.Entities;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Maraudr.User.Infrastructure.Repositories
{
    public class UserRepository(UserContext context) : IUserRepository
    {
        private readonly UserContext _context = context;
        public async Task<AbstractUser?> GetByIdAsync(Guid id)
        {
            return await _context.GetUserByIdAsync(id);
        }

        public async Task<IEnumerable<AbstractUser?>> GetAllAsync()
        {
            return await _context.GetAllUsersAsync();
        }

        public async Task AddAsync(AbstractUser user)
        {
            await _context.AddUserAsync(user) ;
        }
        
        public async Task DeleteAsync(AbstractUser user)
        {
            await _context.DeleteUserAsync(user.Id);
        }
    }
}
