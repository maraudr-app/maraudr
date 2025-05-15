using Application.Security;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Users.User.DeleteUser;

public class DeleteUserHandler(IUserRepository repository): IDeleteUserHandler
{
    public async Task HandleAsync(Guid id,Guid currentUserId)
    {
        var currentUser = await repository.GetByIdAsync(currentUserId);

        if(SecurityChecks.CheckIfUsersMatch(id, currentUserId) || SecurityChecks.CheckIfUserIsAdmin(currentUser))
        {
            throw new InvalidOperationException("Internal error : Can't delete user");
        }
        
        var user = await repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException($"L'utilisateur avec l'ID {id} n'existe pas.");
        }
        await repository.DeleteAsync(user);
    }
}