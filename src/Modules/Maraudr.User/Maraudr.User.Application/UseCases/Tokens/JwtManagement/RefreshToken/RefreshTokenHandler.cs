using Application.UseCases.Tokens.Authentication.RefreshToken;
using Application.UseCases.Tokens.JwtManagement.GenerateAccessToken;
using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.ValueObjects.Tokens;

namespace Application.UseCases.Tokens.JwtManagement.RefreshToken;

public class RefreshTokenHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IGenerateAccessTokenHandler generateAccessTokenHandler,
    IUserRepository userRepository)
    : IRefreshTokenHandler
{
    

    public async Task<RefreshTokenResponse> HandleAsync(string refreshToken)
    {
        var storedToken = await refreshTokenRepository.GetRefreshTokenByTokenAsync(refreshToken);

        if (storedToken == null || storedToken.IsRevoked || !storedToken.IsActive)
            return RefreshTokenResponse.Failed(["Token invalide ou expiré"]);

        var user = await userRepository.GetByIdAsync(storedToken.UserId);
        if (user == null)
            return RefreshTokenResponse.Failed(["Utilisateur introuvable"]);

        // Générer un nouveau access token
        var newAccessToken = await generateAccessTokenHandler.HandleAsync(user);
        var expiresIn = await generateAccessTokenHandler.GetAccessTokenExpirationTime();

        return RefreshTokenResponse.Successful(newAccessToken, expiresIn);
    }
}