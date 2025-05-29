using Application.DTOs.AuthenticationQueriesDto.Requests;
using Maraudr.Authentication.Domain.ValueObjects;

namespace Application.UseCases.Tokens.Authentication.AuthenticateUser;

public interface IAuthenticateUserHandler
{
    public Task<AuthResponse> HandleAsync(LoginRequestDto request);
}