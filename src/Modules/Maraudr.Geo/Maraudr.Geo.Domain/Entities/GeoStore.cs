namespace Maraudr.Geo.Domain.Entities;

public class GeoStore
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid AssociationId { get; init; }
    public ICollection<GeoData> GeoEvents { get; set; } = new List<GeoData>();
}