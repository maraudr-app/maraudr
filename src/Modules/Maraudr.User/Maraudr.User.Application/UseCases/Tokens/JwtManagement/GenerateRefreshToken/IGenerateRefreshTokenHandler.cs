using Maraudr.User.Domain.Entities.Users;

namespace Application.UseCases.Tokens.JwtManagement.GenerateRefreshToken;

public interface IGenerateRefreshTokenHandler
{
    Task<string> HandleAsync(AbstractUser user);
    Task<DateTime> GetRefreshTokenExpirationTime();
}