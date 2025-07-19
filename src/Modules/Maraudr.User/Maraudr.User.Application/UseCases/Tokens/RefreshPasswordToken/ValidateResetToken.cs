using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Tokens.RefreshPasswordToken;

public  interface IValidateResetTokenHandler
{
    public Task<bool> HandleAsync(string token);
    
}



public class ValidateResetTokenHandler(IUserRepository repository) : IValidateResetTokenHandler
{
    public async Task<bool> HandleAsync(string token)
    {
        return  await repository.ValidateResetToken(token);
    }
}