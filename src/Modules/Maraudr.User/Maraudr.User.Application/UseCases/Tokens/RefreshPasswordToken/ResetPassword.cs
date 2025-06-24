using Application.DTOs.AuthenticationQueriesDto.Requests;
using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Infrastructure.Security;

namespace Application.UseCases.Tokens.RefreshPasswordToken;


public interface IResetPasswordHandler
{
    public Task HandleAsync(ConfirmResetRequest request);

}


public class ResetPasswordHandler(IUserRepository repository,IPasswordManager passwordManager):IResetPasswordHandler
{


    public async Task HandleAsync(ConfirmResetRequest request)
    {
        var resetToken = await repository.GetResetToken(request.Token);


        if (resetToken == null)
        {
            throw new Exception("Wrong link");

        }

        var user = await repository.GetByIdAsync(resetToken.UserId);
        if (user == null)
        {
            throw new Exception("Wrong link");
        }
        

        user.PasswordHash = (passwordManager.HashPassword(request.NewPassword));
        await repository.UpdateAsync(user);

        resetToken.IsUsed = true;
        await repository.UpdateResetPasswordTokenAsync(resetToken);
        
        
    
    }
}