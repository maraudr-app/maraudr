namespace Maraudr.Associations.Application.Dtos;

public class AddressDto(string street, string city, string postalCode)
{
    public string Street { get; private set; } = street;
    public string City { get; private set; } = city;
    public string PostalCode { get; private set; } = postalCode;
}