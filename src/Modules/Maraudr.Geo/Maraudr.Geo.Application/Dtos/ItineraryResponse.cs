using System.Text.Json;

namespace Maraudr.Geo.Application.Dtos;

public record ItineraryResponse(
    Guid Id,
    Guid AssociationId,
    Guid EventId,
    double DistanceKm,
    double DurationMinutes,
    double StartLat,
    double StartLng,
    double CenterLat,
    double CenterLng,
    double RadiusKm,
    string GoogleMapsUrl,
    JsonElement GeoJson
);