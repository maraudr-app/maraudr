namespace Maraudr.Geo.Domain.Entities;

public class Itinerary
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AssociationId { get; set; }
    public Guid EventId { get; set; }

    public double StartLat { get; set; }
    public double StartLng { get; set; }

    public double CenterLat { get; set; }
    public double CenterLng { get; set; }

    public double RadiusKm { get; set; }

    public string GeoJson { get; set; } = string.Empty;
    public string GoogleMapsUrl { get; set; } = string.Empty;

    public double DistanceKm { get; set; }
    public double DurationMinutes { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Itinerary() { }

    public Itinerary(Guid associationId, Guid eventId, double startLat, double startLng, double centerLat, double centerLng, double radiusKm,
        string geoJson, string gmapsUrl, double distanceKm, double durationMinutes)
    {
        AssociationId = associationId;
        EventId = eventId;
        StartLat = startLat;
        StartLng = startLng;
        CenterLat = centerLat;
        CenterLng = centerLng;
        RadiusKm = radiusKm;
        GeoJson = geoJson;
        GoogleMapsUrl = gmapsUrl;
        DistanceKm = distanceKm;
        DurationMinutes = durationMinutes;
    }
}
