using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.User.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;

        public UserRepository(UserContext context)
        {
            _context = context;
        }

        public async Task<AbstractUser?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<AbstractUser>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task AddAsync(AbstractUser user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(AbstractUser user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(AbstractUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<AbstractUser>> SearchByNameAsync(string searchTerm)
        {
             return await _context.Users
                .Where(u => u.Firstname.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                            u.Lastname.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }
        
        public async Task<AbstractUser?> GetByEmailAsync(string email)
        {
            var lowerEmail = email.ToLower();
            return await _context.Users
                .AsNoTracking()
                .ToListAsync() 
                .ContinueWith(t => t.Result
                    .FirstOrDefault(u => u.ContactInfo.Email.ToLower() == lowerEmail));
        }
        
    }
}
