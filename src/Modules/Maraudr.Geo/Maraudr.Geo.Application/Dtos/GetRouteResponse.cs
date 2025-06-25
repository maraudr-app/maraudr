namespace Maraudr.Geo.Application.Dtos;

public record GeoRouteResponse(
    List<double[]> Coordinates,
    string GeoJson,
    double Distance, // en mètres
    double Duration, // en secondes
    string GoogleMapsUrl
);