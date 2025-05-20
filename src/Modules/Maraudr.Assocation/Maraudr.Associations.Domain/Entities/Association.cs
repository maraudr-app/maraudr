using Maraudr.Associations.Domain.Siret;

namespace Maraudr.Associations.Domain.Entities;

public class Association
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public List<Guid> Members { get; init; } = [];
    public string? Country { get; init; }
    public string? City { get; init; }
    public string Name { get; init; }
    public SiretNumber? Siret { get; init; }
    public bool IsVerified { get; set; }

    public Association(string name, string city, string country)
    {
        Country = country;
        City = city;
        Name = name ?? throw new ArgumentNullException(nameof(name), "Un nom d'association est requis");
    }
    
    public Association(string name, string city, string country, SiretNumber? siret)
    {
        Country = country;
        City = city;
        Name = name ?? throw new ArgumentNullException(nameof(name), "Un nom d'association est requis");
        Siret = siret;
    }

    public void ValidateAssociationVerification() => IsVerified = true;

    public void AddMembers(List<Guid> members) => Members.AddRange(members);    
    
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