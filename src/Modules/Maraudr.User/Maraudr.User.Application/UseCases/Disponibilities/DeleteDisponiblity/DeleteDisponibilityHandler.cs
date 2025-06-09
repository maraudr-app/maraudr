using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Disponibilities.DeleteDisponiblity;

public class DeleteDisponibilityHandler(IUserRepository repository):IDeleteDisponibilityHandler
{
    public async Task HandleAsync(Guid id,Guid dispoId)
    {
        var user = await repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        user.RemoveAvailability(dispoId);
        await repository.UpdateAsync(user);

    }
}