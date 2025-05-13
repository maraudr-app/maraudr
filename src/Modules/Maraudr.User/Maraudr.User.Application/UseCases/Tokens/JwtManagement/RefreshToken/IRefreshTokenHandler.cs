using Maraudr.User.Domain.ValueObjects.Tokens;

namespace Application.UseCases.Tokens.Authentication.RefreshToken;

public interface IRefreshTokenHandler
{
    public Task<RefreshTokenResponse> HandleAsync(string token);

}