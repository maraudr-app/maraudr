namespace Maraudr.Geo.Application.Dtos;

public record GeoDataResponseWithAddress(
    Guid Id,
    Guid GeoStoreId,
    double Latitude,
    double Longitude,
    string Notes,
    DateTime ObservedAt,
    string? Address
);
