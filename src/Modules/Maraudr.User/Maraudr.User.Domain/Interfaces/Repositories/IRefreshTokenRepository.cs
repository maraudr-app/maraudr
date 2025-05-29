using Maraudr.Authentication.Domain.Entities;
using Maraudr.User.Domain.Entities.Tokens;

namespace Maraudr.User.Domain.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        public Task AddRefreshTokenAsync(RefreshToken refreshToken);


        public Task UpdateRefreshTokenAsync(RefreshToken refreshToken);


        public Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token);


        public Task<List<RefreshToken>> GetActiveRefreshTokensByUserIdAsync(Guid userId);


        public Task RevokeAllUserRefreshTokensAsync(Guid userId, string reason = "Révocation par mesure de sécurité");

        public Task DeleteExpiredRefreshTokensAsync();
        public Task UpdateAsync(RefreshToken token);


    }
}
