using Maraudr.Associations.Domain.Siret;
using Maraudr.Associations.Domain.ValueObjects;

namespace Maraudr.Associations.Domain.Entities;

public class Association
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public List<Guid> Members { get; init; } = [];
    public string? Country { get; set; }
    public string? City { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; } = null!;
    public SiretNumber? Siret { get; set; }

    public Association(string name, string city, string country, SiretNumber? siret, Address address)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        City = city;
        Country = country;
        Siret = siret;
        Address = address;
    }
    
    private Association() { }
    
    public void UpdateInformation(string name, Address newAddress)
    {
        Name = name;
        Address = newAddress;
    }
    
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