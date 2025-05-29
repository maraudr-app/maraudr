using Maraudr.Associations.Application.Dtos;
using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Interfaces;
using Maraudr.Associations.Domain.ValueObjects;

namespace Maraudr.Associations.Application.UseCases.Command;

public interface IUpdateAssociationHandler
{
    Task<Association?> HandleAsync(UpdateAssociationInformationDto association);
}

public class UpdateAssociation(IAssociations repository) : IUpdateAssociationHandler
{
    public async Task<Association?> HandleAsync(UpdateAssociationInformationDto dto)
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
        return existing;
    }
}

