namespace Maraudr.Geo.Domain.Entities;

public class GeoData
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid GeoStoreId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public DateTime ObservedAt { get; init; } = DateTime.UtcNow;
    public string? Notes { get; init; }

    public GeoStore? GeoStore { get; set; }
}