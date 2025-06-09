using Maraudr.Geo.Domain.Entities;

namespace Maraudr.Geo.Domain.Interfaces;

public interface IGeoRepository
{
    Task AddEventAsync(GeoData geoEvent);
    Task<List<GeoData>> GetEventsAsync(Guid associationId, DateTime from);
    Task<GeoStore?> GetGeoStoreByAssociationAsync(Guid associationId);
    Task<GeoStore> CreateGeoStoreAsync(Guid associationId);
}