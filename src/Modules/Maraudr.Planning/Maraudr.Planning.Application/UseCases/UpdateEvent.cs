using Maraudr.Planning.Application.DTOs;
using Maraudr.Planning.Domain.Entities;
using Maraudr.Planning.Domain.Interfaces;

namespace Maraudr.Planning.Application.UseCases;

public interface IUpdateEventHandler
{
    public Task HandleAsync(Guid userId, Guid eventId, UpdateEventRequest request);
}

public class UpdateEventHandler(IPlanningRepository repository):IUpdateEventHandler
{
    public async Task HandleAsync(Guid userId, Guid eventId, UpdateEventRequest request)
    {
        var uEvent = await repository.GetEventByIdAsync(eventId);
    
        if (uEvent.OrganizerdId != userId )
        {
            throw new ArgumentException("You don't have the rights to update this event ");
        }

        uEvent.UpdateFromRequest(request);
        await repository.UpdateEventAsync(uEvent);
        
        
    }
    
    
}

public static class EventExtensions
{
    public static void UpdateFromRequest(this Event eventEntity, UpdateEventRequest request)
    {
        if (request.ParticipantsIds != null)
            eventEntity.ParticipantsIds = request.ParticipantsIds;
        
        if (request.BeginningDate != null)
            eventEntity.BeginningDate = request.BeginningDate.Value;
        
        if (request.EndDate != null)
            eventEntity.EndDate = (DateTime)request.EndDate;
        
        if (!string.IsNullOrEmpty(request.Title))
            eventEntity.Title = request.Title;
        
        if (!string.IsNullOrEmpty(request.Description))
            eventEntity.Description = request.Description;
        
        if (!string.IsNullOrEmpty(request.Location))
            eventEntity.Location = request.Location;
    }
}