using Maraudr.Planning.Domain.Entities;
using Maraudr.Planning.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.Planning.Infrastructure.Repositories;

public class PlanningRepository(PlanningContext context):IPlanningRepository
{
    public async Task AddEventAsync(Event @event)
    {
        await context.Events.AddAsync(@event);
        await context.SaveChangesAsync();
    }

    public async Task<Guid> GetPlanningIdFromAssociationAsync(Guid associationId)
    {
        var planning = await context.Plannings
            .FirstOrDefaultAsync(p => p.AssociationId == associationId);
    
        if (planning == null)
        {
            throw new InvalidOperationException($"Aucun planning trouvé pour l'association avec l'ID {associationId}");
        }
    
        return planning.Id;
    }
    
    public async Task DeleteEventByIdAsync(Guid id)
    {
        var @event = await context.Events.FindAsync(id);
        if (@event == null)
        {
            throw new InvalidOperationException($"Événement avec l'ID {id} non trouvé");
        }
    
        context.Events.Remove(@event);
        await context.SaveChangesAsync();
    }

    public async Task<Event> GetEventByIdAsync(Guid id)
    {
        var @event = await context.Events.FindAsync(id);
        if (@event == null)
        {
            throw new InvalidOperationException($"Événement avec l'ID {id} non trouvé");
        }
    
        return @event;
    }

    public async Task<List<Event>> GetAllUserEventsAsync(Guid associationId, Guid userId)
    {
        var planningId = await GetPlanningIdFromAssociationAsync(associationId);
    
        return await context.Events
            .Where(e => e.PlanningId == planningId && e.ParticipantsIds.Contains(userId))
            .ToListAsync();
    }

    public async Task<List<Event>> GetAllEventsAsync(Guid associationId)
    {
        var planningId = await GetPlanningIdFromAssociationAsync(associationId);
    
        return await context.Events
            .Where(e => e.PlanningId == planningId)
            .ToListAsync();
    }

    public async Task<List<Event>> GetAllUserEventsAsync(Guid userId)
    {
        return await context.Events
            .Where(e => e.ParticipantsIds.Contains(userId))
            .ToListAsync();
    }
    
    public async Task UpdateEventAsync(Event @event)
    {
        context.Events.Update(@event);
        await context.SaveChangesAsync();
    }

}