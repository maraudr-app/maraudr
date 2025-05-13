namespace Maraudr.User.Domain.ValueObjects.Users;

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }

    public Address(string street, string city, string state, string postalCode, string country)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        State = state ?? throw new ArgumentNullException(nameof(state));
        PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
        Country = country ?? throw new ArgumentNullException(nameof(country));
    }

    private Address(
    )
    {
    }

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

    public string GetStreet()
    {
        return Street;
    }

    public string GetCity()
    {
        return City;
    }

    public string GetState()
    {
        return State;
    }

    public string GetPostalCode()
    {
        return PostalCode;
    }

    public string GetCountry()
    {
        return Country;
    }





}