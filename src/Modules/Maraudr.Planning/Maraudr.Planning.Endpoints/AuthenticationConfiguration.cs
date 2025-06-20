using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Maraudr.Planning.Endpoints;


public static class AuthenticationConfiguration
{
    public static void AddAuthenticationServicesForPlanning(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["JWT:Secret"] ?? Environment.GetEnvironmentVariable("Jwt__Secret");
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("La clé secrète JWT n'est pas configurée.");
        }
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSection = configuration.GetSection("JWT");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSection["ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSection["ValidAudience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
    }
}