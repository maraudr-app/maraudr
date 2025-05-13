using Application.DTOs.AuthenticationQueriesDto.Requests;
using Application.UseCases.Tokens.JwtManagement.GenerateAccessToken;
using Application.UseCases.Tokens.JwtManagement.GenerateRefreshToken;
using Maraudr.Authentication.Domain.ValueObjects;
using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Infrastructure.Security;

namespace Application.UseCases.Tokens.Authentication.AuthenticateUser;

public class AuthenticateUserHandler(
    IUserRepository userRepository
    ,IPasswordManager passwordManager,
    IGenerateAccessTokenHandler generateAccessTokenHandler
    //IGenerateRefreshTokenHandler generateRefreshTokenHandler
    )
    : IAuthenticateUserHandler
{

    public async Task<AuthResponse> HandleAsync(LoginRequestDto request)
    {

        var user = await userRepository.GetByEmailAsync(request.Email);
        
        if (user == null)
        {
            return AuthResponse.Failed(["Email ou mot de passe incorrect"]);
        }

        if (!passwordManager.VerifyPassword(user.PasswordHash,request.Password ))
        {
            return AuthResponse.Failed(["Email ou mot de passe incorrect"]);
        }

        // 4. Générer le token JWT
        var token = await generateAccessTokenHandler.HandleAsync(user);
       // var refreshToken = await  generateRefreshTokenHandler.HandleAsync(user);
        var expiresIn = await generateAccessTokenHandler.GetAccessTokenExpirationTime();

 //       return AuthResponse.Successful(token,refreshToken,expiresIn);
        return AuthResponse.Successful(token,"dfdlfdl",2);

        return null;

    }
}