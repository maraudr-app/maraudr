using Maraudr.Planning.Domain.Entities;
using Maraudr.Planning.Domain.Interfaces;

namespace Maraudr.Planning.Application.UseCases;


public interface IGetAllAssociationEventsHandler
{
    public Task<IEnumerable<Event>> HandleAsync(Guid associationId,Guid userId);
}
public class GetAllAssociationEventsHandler(IPlanningRepository repository, IAssociationRepository associationRepository): IGetAllAssociationEventsHandler
{
    public async Task<IEnumerable<Event>> HandleAsync(Guid associationId,Guid userId)
    {
        var isMemberOfAssociation = await associationRepository.IsUserMemberOfAssociationAsync(userId, associationId);
        if (!isMemberOfAssociation)
        {
            throw new UnauthorizedAccessException($"L'utilisateur {userId} n'est pas autorisé à récuperer les événements");

        }
        var exists = await repository.AssociationExistsByIdAsync(associationId);
        if (!exists)
            throw new InvalidOperationException("Association not found");

        var events = await repository.GetAllEventsAsync(associationId);
        return events;
    }
}