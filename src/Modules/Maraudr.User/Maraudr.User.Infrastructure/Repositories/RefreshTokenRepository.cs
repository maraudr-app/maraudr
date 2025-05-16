using Maraudr.Authentication.Domain.Entities;
using Maraudr.User.Domain.Entities.Tokens;
using Maraudr.User.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.User.Infrastructure.Repositories;

public class RefreshTokenRepository(UserContext context):IRefreshTokenRepository
{
    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
    {
        context.RefreshTokens.Update(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token)
    {
        return await context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == token);
    }

    public async Task<List<RefreshToken>> GetActiveRefreshTokensByUserIdAsync(Guid userId)
    {
        return await context.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked && r.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task RevokeAllUserRefreshTokensAsync(Guid userId, string reason = "Révocation par mesure de sécurité")
    {
        var userTokens = await context.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked)
            .ToListAsync();
    
        foreach (var token in userTokens)
        {
            token.Revoke(reason);
        }
    
        await context.SaveChangesAsync();
    }

    public async Task DeleteExpiredRefreshTokensAsync()
    {
        var expiredTokens = await context.RefreshTokens
            .Where(r => r.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();
    
        context.RefreshTokens.RemoveRange(expiredTokens);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        context.RefreshTokens.Update(token);
        await context.SaveChangesAsync();
    }
}