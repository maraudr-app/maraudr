namespace Maraudr.Geo.Application.Dtos;

public record GeoRouteResponse(
    List<double[]> Coordinates,
    string GeoJson,
    double Distance, 
    double Duration,
    string GoogleMapsUrl
);