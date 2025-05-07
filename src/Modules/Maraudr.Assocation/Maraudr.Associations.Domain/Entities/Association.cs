using Maraudr.Associations.Domain.Siret;

namespace Maraudr.Associations.Domain.Entities;

public class Association(string name, string city, string country, List<Guid> members, SiretNumber? siret = null)
{
    private Guid Id { get; } = Guid.NewGuid();
    public DateTime CreationDate { get; } = DateTime.Now;
    public List<Guid> Members { get; } = members;
    public string Country { get; } = country ?? throw new ArgumentNullException(nameof(country));
    public string City { get; } = city ?? throw new ArgumentNullException(nameof(city));
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));
    public SiretNumber? Siret { get; } = siret;
    public bool IsValidSiret { get; } = siret is not null;

    private bool Equals(Association other)
    {
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is Association other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}