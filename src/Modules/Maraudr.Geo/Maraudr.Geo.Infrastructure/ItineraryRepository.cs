using Maraudr.Geo.Domain.Entities;
using Maraudr.Geo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.Geo.Infrastructure;

public class ItineraryRepository(GeoContext context) : IItineraryRepository
{
    public async Task AddAsync(Itinerary itinerary)
    {
        context.Itineraries.Add(itinerary);
        await context.SaveChangesAsync();
    }

    public async Task<Itinerary?> GetByIdAsync(Guid id)
        => await context.Itineraries.FindAsync(id);

    public async Task<List<Itinerary>> GetByAssociationIdAsync(Guid associationId)
        => await context.Itineraries
            .Where(i => i.AssociationId == associationId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
}
