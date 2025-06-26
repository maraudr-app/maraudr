namespace Maraudr.Geo.Application.Dtos;

public record CreateItineraryRequest(
    Guid AssociationId,
    Guid EventId,
    double CenterLat,
    double CenterLng,
    double RadiusKm,
    double StartLat,
    double StartLng
);