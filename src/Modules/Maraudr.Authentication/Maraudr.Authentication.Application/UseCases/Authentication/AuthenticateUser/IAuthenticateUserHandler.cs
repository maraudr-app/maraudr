using Maraudr.Authentication.Application.DTOs.Requests;
using Maraudr.Authentication.Domain.ValueObjects;

namespace Maraudr.Authentication.Application.UseCases.Authentication.AuthenticateUser;

public interface IAuthenticateUserHandler
{
    public Task<AuthResponse> HandleAsync(LoginRequestDto request);
}