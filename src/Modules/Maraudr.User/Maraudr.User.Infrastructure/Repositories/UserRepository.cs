using Maraudr.Authentication.Domain.Entities;
using Maraudr.User.Domain.Entities.Tokens;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.User.Infrastructure.Repositories
{
    public class UserRepository(UserContext context) : IUserRepository
    {
        public async Task<AbstractUser?> GetByIdAsync(Guid id)
        {
            var user = await context.Users.FindAsync(id);
            if (user is Manager)
            {
                await context.Entry(user).Collection("Team").LoadAsync();
            }
            return user;
        }

        public async Task<IEnumerable<AbstractUser>> GetAllAsync()
        {
            return await context.Users.ToListAsync();
        }

        public async Task AddAsync(AbstractUser user)
        {
            await context.Users.AddAsync(user);
            
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(AbstractUser user)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
        public async Task UpdateAsync(AbstractUser user)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
        public async Task<IEnumerable<AbstractUser>> SearchByNameAsync(string searchTerm)
        {
             return await context.Users
                .Where(u => u.Firstname.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                            u.Lastname.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }
        
        public async Task<AbstractUser?> GetByEmailAsync(string email)
        {
            var lowerEmail = email.ToLower();
    
            return await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactInfo.Email.ToLower() == lowerEmail);
        }
        
        public async Task<RefreshToken?> GetRefreshTokenByUserIdAsync(Guid id)
        {
            return await context.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.UserId == id);
        }
        
    }
}
