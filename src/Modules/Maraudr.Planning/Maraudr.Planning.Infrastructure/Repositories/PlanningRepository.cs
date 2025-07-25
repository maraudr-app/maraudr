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
    
    public async Task<Guid> GetAssociationIdFromPlanningIdAsync(Guid planningId)
    {
        var planning = await context.Plannings
            .FirstOrDefaultAsync(p => p.Id == planningId);

        if (planning == null)
        {
            throw new InvalidOperationException($"Aucun planning trouvé avec l'ID {planningId}");
        }

        return planning.AssociationId;
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
    

    public async Task<List<Event>> GetAllEventsAsync(Guid associationId)
    {
        var planningId = await GetPlanningIdFromAssociationAsync(associationId);
    
        return await context.Events
            .Where(e => e.PlanningId == planningId)
            .ToListAsync();
    }
    
    
    public async Task<List<Event>> GetAllEventsAsync()
    {
        return await context.Events.AsNoTracking().ToListAsync();  
    }
   

    public async Task<bool> AssociationExistsByIdAsync(Guid associationId)
    {
        return await context.Plannings.AnyAsync(p => p.AssociationId == associationId);
    }
    public async Task UpdateEventAsync(Event @event)
    {
        context.Events.Update(@event);
        await context.SaveChangesAsync();
    }
    
    public async Task CreatePlanningAsync(Domain.Entities.Planning planning)
    {
        await context.Plannings.AddAsync(planning);
        await context.SaveChangesAsync();
    }



}