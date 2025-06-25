using Maraudr.Geo.Domain.Entities;

namespace Maraudr.Geo.Domain.Interfaces;

public interface IGeoRepository
{
    Task<List<GeoData>> GetEventsAsync(Guid associationId, DateTime from);
    Task<(string? RouteGeoJson, string GoogleMapsUrl)> GetRouteAsync(Guid associationId, double lat, double lng, double radiusKm);
    Task AddEventAsync(GeoData geoEvent);
    Task<GeoStore?> GetGeoStoreByAssociationAsync(Guid associationId);
    Task<GeoStore> CreateGeoStoreAsync(Guid associationId);
}