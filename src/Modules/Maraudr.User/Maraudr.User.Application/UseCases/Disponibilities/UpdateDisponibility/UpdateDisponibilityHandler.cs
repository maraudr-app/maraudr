using Application.DTOs.DisponibilitiesQueriesDtos.Requests;
using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.ValueObjects.Users;

namespace Application.UseCases.Disponibilities.UpdateDisponibility;

public class UpdateDisponibilityHandler(IUserRepository repository):IUpdateDisponibilityHandler
{
    public async Task HandleAsync(Guid dispoId, Guid id, UpdateDisponiblityRequest request)
    {
        var user = await repository.GetByIdAsync(id);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        user.UpdateDisponibility(dispoId, request.Start, request.End);
        await repository.UpdateAsync(user);

    }
}