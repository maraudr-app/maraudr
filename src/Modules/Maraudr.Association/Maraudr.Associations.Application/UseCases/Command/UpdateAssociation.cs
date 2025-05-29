using Maraudr.Associations.Application.Dtos;
using Maraudr.Associations.Domain.Interfaces;
using Maraudr.Associations.Domain.ValueObjects;

namespace Maraudr.Associations.Application.UseCases.Command;

public interface IUpdateAssociationHandler
{
    Task<AssociationDto?> HandleAsync(UpdateAssociationInformationDto dto);
}

public class UpdateAssociation(IAssociations repository) : IUpdateAssociationHandler
{
    public async Task<AssociationDto?> HandleAsync(UpdateAssociationInformationDto dto)
    {
        var existing = await repository.GetAssociation(dto.Id);
        if (existing is null)
            return null;

        var newAddress = new Address(
            dto.AddressDto.Street,
            dto.AddressDto.City,
            dto.AddressDto.PostalCode,
            existing.Country ?? "France"
        );

        existing.UpdateInformation(dto.Name, newAddress);

        await repository.UpdateAssociation(existing);

        return new AssociationDto(
            existing.Id,
            existing.Name,
            new AddressDto(
                existing.Address.Street,
                existing.Address.City,
                existing.Address.PostalCode
            )
        );
    }
}