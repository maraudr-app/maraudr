namespace Maraudr.Associations.Application.Dtos;

public class AssociationDto(Guid id, string name, AddressDto address)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public AddressDto Address { get; set; } = address;
}