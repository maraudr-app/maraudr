public class Itinerary
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid EventId { get; private set; }
    public Guid AssociationId { get; private set; }

    public string GeoJson { get; private set; }
    public string GoogleMapsUrl { get; private set; }

    public double DistanceKm { get; private set; }
    public double DurationMinutes { get; private set; }

    public double StartLat { get; private set; }
    public double StartLng { get; private set; }

    public double CenterLat { get; private set; }
    public double CenterLng { get; private set; }

    public double RadiusKm { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public Itinerary(Guid eventId, Guid associationId, double distanceKm, double durationMinutes,
        string geoJson, string googleMapsUrl,
        double startLat, double startLng, double centerLat, double centerLng, double radiusKm)
    {
        EventId = eventId;
        AssociationId = associationId;
        DistanceKm = distanceKm;
        DurationMinutes = durationMinutes;
        GeoJson = geoJson;
        GoogleMapsUrl = googleMapsUrl;
        StartLat = startLat;
        StartLng = startLng;
        CenterLat = centerLat;
        CenterLng = centerLng;
        RadiusKm = radiusKm;
    }

    private Itinerary() {}
}