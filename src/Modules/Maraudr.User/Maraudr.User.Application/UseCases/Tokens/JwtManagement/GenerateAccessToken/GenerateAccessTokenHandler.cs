using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Maraudr.User.Domain.Entities.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.UseCases.Tokens.JwtManagement.GenerateAccessToken;

public class GenerateAccessTokenHandler(IConfiguration configuration): IGenerateAccessTokenHandler
{
    public Task<string> HandleAsync(AbstractUser user)
    {
        var jwtSettings = configuration.GetSection("JWT");
        var secretKey = jwtSettings["Secret"];
        var issuer = jwtSettings["ValidIssuer"];
        var audience = jwtSettings["ValidAudience"];
        var expirationMinutes = Convert.ToInt32(jwtSettings["TokenValidityInMinutes"]);

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("La clé secrète JWT n'est pas configurée.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), 
            new Claim(JwtRegisteredClaimNames.Email, user.ContactInfo.Email), 
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Task.FromResult(tokenHandler.WriteToken(token));
        
    }

    public Task<DateTime> GetAccessTokenExpirationTime()
    {
        var expirationMinutes = Convert.ToInt32(configuration["JwtSettings:AccessTokenExpirationMinutes"]);
        return Task.FromResult(DateTime.UtcNow.AddMinutes(expirationMinutes));    }
}