namespace Maraudr.Geo.Application.Dtos;

public record CreateGeoDataRequest(
    Guid AssociationId,
    double Latitude,
    double Longitude,
    string? Notes
);