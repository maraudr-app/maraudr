using Maraudr.Authentication.Application.DTOs.Requests;
using Maraudr.Authentication.Application.UseCases.Authentication.AuthenticateUser;
using Maraudr.Authentication.Application.UseCases.JwtManagement.GenerateAccessToken;
using Maraudr.Authentication.Application.UseCases.JwtManagement.GenerateRefreshToken;
using Maraudr.Authentication.Domain.ValueObjects;
using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.ValueObjects;

namespace Maraudr.Authentication.Application.UseCases.Authentication.AuthenticateUser;

public class AuthenticateUserHandler(
    IUserRepository userRepository,
    IGenerateAccessTokenHandler generateAccessTokenHandler,
    //IPasswordHashService passwordHashService,
    IGenerateRefreshTokenHandler generateRefreshTokenHandler)
    : IAuthenticateUserHandler
{

    public async Task<AuthResponse> HandleAsync(LoginRequestDto request)
    {

        var user = await userRepository.GetByEmailAsync(request.Email);
        
        // 2. Vérifier si l'utilisateur existe
        if (user == null)
        {
            return AuthResponse.Failed(["Email ou mot de passe incorrect"]);
        }

        // 3. Vérifier le mot de passe
        if (await !passwordHashService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return AuthResponse.Failed(["Email ou mot de passe incorrect"]);

        }

        // 4. Générer le token JWT
        var token = await generateAccessTokenHandler.HandleAsync(user);
        var refreshToken = await  generateRefreshTokenHandler.HandleAsync(user);
        var expiresIn = await generateAccessTokenHandler.GetAccessTokenExpirationTime();

        return AuthResponse.Successful(token,refreshToken,expiresIn);
       
    }
}