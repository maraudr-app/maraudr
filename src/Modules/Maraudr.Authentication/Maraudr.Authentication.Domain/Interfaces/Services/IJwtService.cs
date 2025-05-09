using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Maraudr.Authentication.Domain.Interfaces.Services
{
    internal interface IJwtService
    {
        string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
        (Guid UserId, string Email) GetUserInfoFromToken(string token);
    }
}
