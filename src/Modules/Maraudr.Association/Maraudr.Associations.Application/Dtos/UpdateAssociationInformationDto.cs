namespace Maraudr.Associations.Application.Dtos;

public class UpdateAssociationInformationDto(Guid id, string name, AddressDto addressDto)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public AddressDto AddressDto { get; set; } = addressDto;
}
