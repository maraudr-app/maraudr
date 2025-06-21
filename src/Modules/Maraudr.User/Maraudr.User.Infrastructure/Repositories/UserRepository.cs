
using Maraudr.Authentication.Domain.Entities;
using Maraudr.User.Domain.Entities.Tokens;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.ValueObjects.Users;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.User.Infrastructure.Repositories
{
    public class UserRepository(UserContext context) : IUserRepository
    {
        public async Task<AbstractUser?> GetByIdAsync(Guid id)
        {
            var user = await context.Users
                .Include(u => u.Disponibilities)  
                .FirstOrDefaultAsync(u => u.Id == id);
    
            if (user is Manager)
            {
                var cUser = (Manager)user;
                await context.Entry(cUser).Collection(m => m.EFTeam).LoadAsync();

            }
    
            return user;
        }

        public async Task<IEnumerable<AbstractUser>> GetAllAsync()
        {
            return await context.Users.ToListAsync();
        }

        public async Task AddAsync(AbstractUser user)
        {
            if (user is Manager && user.Role != Role.Manager)
            {
                user.Role = Role.Manager;
            }
            else if (user is Domain.Entities.Users.User && user.Role != Role.Member)
            {
                user.Role = Role.Member;
            }
    
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
                 .Where(u => u.Firstname.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) 
                             || u.Lastname.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
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
        
        
        public async Task<IEnumerable<Disponibility>> GetDisponibilitiesByAssociationIdAsync(Guid associationId)
        {
            return await context.Disponibilities
                .Where(d => d.AssociationId == associationId)
                .ToListAsync();
        }
        
    }
}
