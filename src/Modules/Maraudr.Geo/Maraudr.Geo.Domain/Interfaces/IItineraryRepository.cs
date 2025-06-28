using Maraudr.Geo.Domain.Entities;

namespace Maraudr.Geo.Domain.Interfaces;

public interface IItineraryRepository
{
    Task AddAsync(Itinerary itinerary);
    Task<Itinerary?> GetByIdAsync(Guid id);
    Task<List<Itinerary>> GetByAssociationIdAsync(Guid associationId);
}
