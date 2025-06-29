using Maraudr.Planning.Domain.Entities;

namespace Maraudr.Planning.Domain.Interfaces;

public interface IPlanningRepository
{
    public Task AddEventAsync(Event @event);
    public Task<Guid> GetPlanningIdFromAssociationAsync(Guid associationId);
    public Task<Guid> GetAssociationIdFromPlanningIdAsync(Guid planningId);
    public Task UpdateEventAsync(Event @event);

    public Task DeleteEventByIdAsync(Guid id);
    public Task<Event> GetEventByIdAsync(Guid id);
    public Task<List<Event>> GetAllEventsAsync(Guid associationId);

    public Task<List<Event>> GetAllEventsAsync();
    public Task<bool> AssociationExistsByIdAsync(Guid associationId);

    public Task CreatePlanningAsync(Domain.Entities.Planning planning);




}