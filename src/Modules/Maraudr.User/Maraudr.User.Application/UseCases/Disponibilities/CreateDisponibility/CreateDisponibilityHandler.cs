using Application.DTOs.DisponibilitiesQueriesDtos.Requests;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Disponibilities.CreateDisponibility;

public class CreateDisponibilityHandler(IUserRepository repository):ICreateDisponibilityHandler
{
    public async Task HandleAsync(Guid id, CreateDisponiblityRequest request)
    {
        var user = await repository.GetByIdAsync(id);
        
        if (user == null)
            throw new ArgumentException("Utilisateur non trouvé");
            
        user.AddDisponiblity(request.Start, request.End);
        
        var newDisponibilityId = user.Disponibilities.Last().Id;
        
        await repository.UpdateAsync(user);
        
     
    }
}