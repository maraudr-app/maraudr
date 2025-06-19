using System.Security.AccessControl;
using Maraudr.Planning.Domain.Interfaces;
using Maraudr.Planning.Domain.ValueObjects;

namespace Maraudr.Planning.Application.UseCases;



public interface IDeleteAnEventHandler
{
    public Task HandleAsync(Guid userId,Role userRole, Guid eventId);
}
public class DeleteAnEventHandler(IPlanningRepository repository,IAssociationRepository associationRepository): IDeleteAnEventHandler
{
    public async Task HandleAsync(Guid userId,Role userRole,Guid eventId)
    {
        var @event = await repository.GetEventByIdAsync(eventId);
        if (@event == null)
        {
            throw new ArgumentException($" The event {eventId} doesn't exist");

        }

        var associationId = await repository.GetAssociationIdFromPlanningIdAsync(@event.PlanningId);
        var isMemberOfAssociation = await associationRepository.IsUserMemberOfAssociationAsync(userId, associationId);
        
        if (@event.OrganizerdId != userId && userRole != Role.Manager || !isMemberOfAssociation)
        {
            throw new UnauthorizedAccessException($"L'utilisateur {userId} n'est pas autorisé à supprimer cet événement");
        }

        await repository.DeleteEventByIdAsync(eventId);
    }
}