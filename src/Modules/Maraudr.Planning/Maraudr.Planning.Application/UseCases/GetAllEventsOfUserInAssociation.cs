using Maraudr.Planning.Domain.Entities;
using Maraudr.Planning.Domain.Interfaces;

namespace Maraudr.Planning.Application.UseCases;

public interface IGetAllEventsOfUserInAssociationHandler
{ 
    Task<IEnumerable<Event>> HandleAsync(Guid userId,Guid associationId);
}   
public class GetAllEventsOfUserInAssociationHandler(IPlanningRepository repository, IAssociationRepository associationRepository):IGetAllEventsOfUserInAssociationHandler
{
    public async Task<IEnumerable<Event>> HandleAsync(Guid userId,Guid associationId)
    {
        var isMemberOfAssociation = await associationRepository.IsUserMemberOfAssociationAsync(userId, associationId);
        if (!isMemberOfAssociation)
        {
            throw new UnauthorizedAccessException($"L'utilisateur {userId} n'est pas autorisé à récuperer les événements");

        }
        
        var events =  await repository.GetAllEventsAsync(associationId);
        return events.Where(e => 
            e.OrganizerdId == userId || 
            (e.ParticipantsIds.Contains(userId))
        ).ToList();
    }
}