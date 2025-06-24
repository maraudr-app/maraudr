using Maraudr.Associations.Domain.Siret;
using Maraudr.Associations.Domain.ValueObjects;

namespace Maraudr.Associations.Domain.Entities;

public class Association
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ManagerId { get; set; }
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
    
    public Association(Guid id, string name)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
    
    private Association() { }
    
    public void UpdateInformation(string name, Address newAddress)
    {
        Name = name;
        Address = newAddress;
    }
    
    public void AddMember(Guid userId)
    {
        if (Members.Contains(userId))
            throw new InvalidOperationException("User already member of the association.");

        Members.Add(userId);
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