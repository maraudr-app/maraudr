using System.Security.Cryptography;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;

namespace Application.UseCases.Tokens.JwtManagement.GenerateRefreshToken;

public class GenerateRefreshTokenHandler(IRefreshTokenRepository repository,IConfiguration configuration) : IGenerateRefreshTokenHandler
{
    public async Task<string> HandleAsync(AbstractUser user)
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        var refreshToken = Convert.ToBase64String(randomNumber);
        var expirationDays = Convert.ToInt32(configuration["JWT:RefreshTokenValidityInDays"]);
       
        await repository.AddRefreshTokenAsync(new Maraudr.User.Domain.Entities.Tokens.RefreshToken(user.Id,refreshToken, DateTime.UtcNow.AddDays(expirationDays)));
        
        return refreshToken;
    }

    public Task<DateTime> GetRefreshTokenExpirationTime()
    {
        var expirationDays = Convert.ToInt32(configuration["JWT:RefreshTokenValidityInDays"]);
        return Task.FromResult(DateTime.UtcNow.AddDays(expirationDays));
    }
}