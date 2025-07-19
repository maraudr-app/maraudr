using Maraudr.Geo.Application.Dtos;
using Maraudr.Geo.Domain.Interfaces;

namespace Maraudr.Geo.Application.UseCases;

public interface IGetGeoRouteHandler
{
    Task<GeoRouteResponse?> HandleAsync(
        Guid associationId,
        Guid eventId,
        double centerLat,
        double centerLng,
        double radiusKm,
        double startLat,
        double startLng);
}

public class GetGeoRouteHandler(IGeoRepository repository) : IGetGeoRouteHandler
{
    public async Task<GeoRouteResponse?> HandleAsync(
        Guid associationId,
        Guid eventId,
        double centerLat,
        double centerLng,
        double radiusKm,
        double startLat,
        double startLng)
    {
        var (coordinates, geoJson, distance, duration, gmapsUrl) =
            await repository.GetRouteAsync(associationId, eventId, centerLat, centerLng, radiusKm, startLat, startLng);

        return coordinates.Count == 0 ? null : new GeoRouteResponse(coordinates, geoJson, distance, duration, gmapsUrl);
    }
}

