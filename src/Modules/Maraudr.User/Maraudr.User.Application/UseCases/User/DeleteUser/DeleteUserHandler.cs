using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.User.DeleteUser;

public class DeleteUserHandler(IUserRepository repository): IDeleteUserHandler
{
    public async Task HandleAsync(Guid id)
    {
        var user = await repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException($"L'utilisateur avec l'ID {id} n'existe pas.");
        }
        await repository.DeleteAsync(user);
    }
}