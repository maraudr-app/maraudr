namespace Maraudr.User.Domain.ValueObjects;

public class Address
{
    private string Street { get;  set; }
    private string City { get;  set; }
    private string State { get;  set; }
    private string PostalCode { get;  set; }
    private string Country { get;  set; }

    public Address(string street, string city, string state, string postalCode, string country)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state ?? throw new ArgumentNullException(nameof(state));
        PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
        Country = country ?? throw new ArgumentNullException(nameof(country));
    }

    private Address(
        ) { }

    public override string ToString()
    {
        return $"{Street}, {City}, {State} {PostalCode}, {Country}";
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (GetType() != obj.GetType()) return false;

        var other = (Address)obj;
        return Street == other.Street &&
               City == other.City &&
               State == other.State &&
               PostalCode == other.PostalCode &&
               Country == other.Country;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, City, State, PostalCode, Country);
    }

  
    
    
}