using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.ValueObjects.Users;

namespace Application.UseCases.Disponibilities.GetAllAssociationUsersDipsonibilities;

public class GetAllAssociationUsersDipsonibilitiesHandler(IUserRepository userRepository,IAssociationRepository associationRepository)
    :IGetAllAssociationUsersDipsonibilitiesHandler
{
    public async Task<IEnumerable<Disponibility>> HandleAsync(Guid userId, Guid associationId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        
        if (user == null)
            throw new ArgumentException("Utilisateur non trouvé");
      
        var isMember = await associationRepository.IsUserMemberOfAssociationAsync(userId,associationId);
        Console.WriteLine(isMember);
        if (!isMember)
        {
            throw new ArgumentException("L'utilisateur n'est pas membre de l'association");

        }

        return await userRepository.GetDisponibilitiesByAssociationIdAsync(associationId);

    }
}