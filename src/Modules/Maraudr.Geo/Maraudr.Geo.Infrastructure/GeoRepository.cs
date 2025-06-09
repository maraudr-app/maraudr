using Maraudr.Geo.Domain.Entities;
using Maraudr.Geo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Maraudr.Geo.Infrastructure;

public class GeoRepository(GeoContext context) : IGeoRepository
{
    public async Task AddEventAsync(GeoData geoEvent)
    {
        context.GeoEvents.Add(geoEvent);
        await context.SaveChangesAsync();
    }

    public async Task<List<GeoData>> GetEventsAsync(Guid associationId, DateTime from)
    {
        return await context.GeoEvents
            .Where(e => e.GeoStore!.AssociationId == associationId && e.ObservedAt >= from)
            .ToListAsync();
    }

    public async Task<GeoStore?> GetGeoStoreByAssociationAsync(Guid associationId)
    {
        return await context.GeoStores.FirstOrDefaultAsync(x => x.AssociationId == associationId);
    }

    public async Task<GeoStore> CreateGeoStoreAsync(Guid associationId)
    {
        var store = new GeoStore { AssociationId = associationId };
        context.GeoStores.Add(store);
        await context.SaveChangesAsync();
        return store;
    }
}
