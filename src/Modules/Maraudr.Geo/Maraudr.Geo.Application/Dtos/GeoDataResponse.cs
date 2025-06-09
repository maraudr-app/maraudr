namespace Maraudr.Geo.Application.Dtos;

public record GeoDataResponse(Guid Id, Guid GeoStoreId, double Latitude, double Longitude, string Notes, DateTime ObservedAt);
