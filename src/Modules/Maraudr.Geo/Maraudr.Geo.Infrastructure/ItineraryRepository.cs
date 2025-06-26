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

    public async Task<Itinerary?> GetByEventIdAsync(Guid eventId)
    {
        return await context.Itineraries.FirstOrDefaultAsync(i => i.EventId == eventId);
    }
}