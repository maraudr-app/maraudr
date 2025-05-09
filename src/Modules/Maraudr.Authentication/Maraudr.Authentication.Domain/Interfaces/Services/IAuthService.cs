using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Maraudr.Authentication.Application.DTOs.Responses;
using Maraudr.Authentication.Domain.ValueObjects;

namespace Maraudr.Authentication.Domain.Interfaces.Services
{
    public interface IAuthService
    {
            Task<AuthResponse> AuthenticateUserAsync(string email, string password); // TODO : create DTO
            Task<RegisterResponse> RegisterUserAsync(RegisterRequestDto request); // todo: create DTO
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
            Task LogoutUserAsync(string userId);
            Task<AuthResponse> HandleGoogleAuthCallbackAsync(HttpContext context);
            Task SendPasswordResetLinkAsync(string email);
            Task<PasswordResetResponse> ResetPasswordAsync(string token, string newPassword);
            Task RevokeUserTokensAsync(Guid userId);
        //Task<IEnumerable<SessionInfo>> GetActiveSessionsAsync();

       
    }
}

