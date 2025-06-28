using Maraudr.Geo.Domain.Entities;

namespace Maraudr.Geo.Domain.Interfaces;

public interface IGeoRepository
{
    Task<List<GeoData>> GetEventsAsync(Guid associationId, DateTime from);
    Task<(List<double[]> Coordinates, string GeoJson, double Distance, double Duration, string GoogleMapsUrl)>
        GetRouteAsync(Guid associationId, Guid eventId, double centerLat, double centerLng, double radiusKm, double startLat, double startLng);
    Task AddEventAsync(GeoData geoEvent);
    Task<GeoStore?> GetGeoStoreByAssociationAsync(Guid associationId);
    Task<GeoStore> CreateGeoStoreAsync(Guid associationId);
}