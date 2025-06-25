using Maraudr.Geo.Application.Dtos;
using Maraudr.Geo.Domain.Interfaces;

namespace Maraudr.Geo.Application.UseCases;

public interface IGetGeoRouteHandler
{
    Task<GeoRouteResponse?> HandleAsync(Guid associationId, double latitude, double longitude, double radiusKm);
}


public class GetGeoRouteHandler(IGeoRepository repository) : IGetGeoRouteHandler
{
    
    public async Task<GeoRouteResponse?> HandleAsync(Guid associationId, double latitude, double longitude, double radiusKm)
    {
        var (geoJson, gmapsUrl) = await repository.GetRouteAsync(associationId, latitude, longitude, radiusKm);

        return geoJson is null ? null : new GeoRouteResponse(geoJson, gmapsUrl);
    }
}
