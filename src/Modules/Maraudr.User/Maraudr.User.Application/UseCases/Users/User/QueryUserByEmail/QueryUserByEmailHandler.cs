using Application.Security;
using Maraudr.User.Domain.Entities.Users;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Users.User.QueryUserByEmail;

public class QueryUserByEmailHandler(IUserRepository repository): IQueryUserByEmailHandler
{
    public async Task<AbstractUser?> HandleAsync(string email,string currentUserEmail)
    {
        var currentUser = await repository.GetByEmailAsync(email);
        
        if(SecurityChecks.CheckIfEmailsMatch(email, currentUserEmail) || SecurityChecks.CheckIfUserIsAdmin(currentUser))
        {
            throw new InvalidOperationException("Internal error : Can't update user");
        }
        return await repository.GetByEmailAsync(email);
    }
}