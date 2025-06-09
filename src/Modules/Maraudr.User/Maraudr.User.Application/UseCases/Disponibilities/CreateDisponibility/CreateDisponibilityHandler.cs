using Application.DTOs.DisponibilitiesQueriesDtos.Requests;
using Maraudr.User.Domain.Interfaces.Repositories;

namespace Application.UseCases.Disponibilities.CreateDisponibility;

public class CreateDisponibilityHandler(IUserRepository repository, IAssociationRepository associationRepository) 
    : ICreateDisponibilityHandler
{
    public async Task HandleAsync(Guid id, CreateDisponiblityRequest request)
    {
        var user = await repository.GetByIdAsync(id);
        
        if (user == null)
            throw new ArgumentException("Utilisateur non trouvé");
      
        var isMember = await associationRepository.IsUserMemberOfAssociationAsync(id,request.AssociationId);
        Console.WriteLine(isMember);
        if (!isMember)
        {
            throw new ArgumentException("L'utilisateur n'est pas membre de l'association");

        }
        user.AddDisponiblity(request.Start, request.End, request.AssociationId);
        await repository.UpdateAsync(user);        
       
      
    }
}