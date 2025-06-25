
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

        public async Task InvalidateExistingTokensAsync(Guid userId)
        {
            var existingTokens = await context.PasswordResetTokens
                .Where(t => t.UserId == userId && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in existingTokens)
            {
                token.IsUsed = true;
            }

            await context.SaveChangesAsync();
        }

        public async Task SaveResetToken(PasswordResetToken token)
        {
            context.PasswordResetTokens.Add(token);
            await context.SaveChangesAsync();

        }

        public async Task<bool> ValidateResetToken(string token)
        {
            var resetToken = await context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == token &&
                                          !t.IsUsed &&
                                          t.ExpiresAt > DateTime.UtcNow);

            return resetToken != null;
        }

        public async Task<PasswordResetToken?> GetResetToken(string token)
        {
            var resetToken = await context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == token &&
                                          !t.IsUsed &&
                                          t.ExpiresAt > DateTime.UtcNow);

            return resetToken;
        }


        public async Task UpdateResetPasswordTokenAsync(PasswordResetToken token)
        {
            context.PasswordResetTokens.Update(token);
            await context.SaveChangesAsync();

        }
        
        
        public async Task InvalidateExistingInvitationsAsync(string email)
        {
            var existingInvitations = await context.InvitationTokens
                .Where(i => i.InvitedEmail.ToLower() == email.ToLower() && 
                            !i.IsUsed && 
                            i.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var invitation in existingInvitations)
            {
                invitation.IsUsed = true;
            }

            await context.SaveChangesAsync();
        }

        public async Task AddInvitationToken(InvitationToken token)
        {
            context.InvitationTokens.Add(token);
            await context.SaveChangesAsync();
        }

        public async Task<Guid> GetManagerIdByInvitationTokenAsync(string token)
        {
            var invitation = await context.InvitationTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Token == token && !i.IsUsed && i.ExpiresAt > DateTime.UtcNow);

            if (invitation == null)
            {
                throw new InvalidOperationException("Le token d'invitation est invalide ou a expiré.");
            }

            return invitation.InvitedByUserId;
        }
    }
}
    
    
       

