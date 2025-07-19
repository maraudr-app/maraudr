using System.Security.Cryptography;
using Maraudr.User.Domain.Entities.Tokens;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Tokens.RefreshPasswordToken;

public interface IInitiatePasswordResetAsync
{
    public Task<bool> HandleAsync(string email);

}


public class InitiatePasswordResetAsync(IUserRepository userRepository, IMailSenderRepository mailRepository)
    : IInitiatePasswordResetAsync
{


    public async Task<bool> HandleAsync(string email)
    {
            var user = await userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return true; 
            }

            await userRepository.InvalidateExistingTokensAsync(user.Id);

            var token = GenerateSecureToken();

            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1), 
            };

            await userRepository.SaveResetToken(resetToken);
            
            Console.WriteLine("Email sent to : " + user.ContactInfo.Email);

            await mailRepository.SendResetEmailAsync(user.Firstname,user.ContactInfo.Email, token);

            return true;
        }
        
    
    
   

    private string GenerateSecureToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[64]; 
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
}