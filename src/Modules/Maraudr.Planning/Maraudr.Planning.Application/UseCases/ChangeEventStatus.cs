using Maraudr.Planning.Domain.Entities;
using Maraudr.Planning.Domain.Interfaces;
using Maraudr.Planning.Domain.ValueObjects;

namespace Maraudr.Planning.Application.UseCases;


public interface IChangeEventStatusHandler
{
    public Task HandleAsync(Guid userId,Guid id, Status status);
}
public class ChangeEventStatusHandler(IPlanningRepository repository): IChangeEventStatusHandler
{
    public async Task HandleAsync(Guid userId, Guid id, Status status)
    {
        var uEvent = await repository.GetEventByIdAsync(id);
    
        if (uEvent.OrganizerdId != userId && !uEvent.ParticipantsIds.Contains(userId))
        {
            throw new ArgumentException("You don't have the rights to update this event you're not in it");
        }

        if (status == Status.CANCELED && uEvent.OrganizerdId != userId)
        {
            throw new ArgumentException("Only organizer can cancel it");
        }

        uEvent.ChangeStatus(status);
        await repository.UpdateEventAsync(uEvent);
    }
}