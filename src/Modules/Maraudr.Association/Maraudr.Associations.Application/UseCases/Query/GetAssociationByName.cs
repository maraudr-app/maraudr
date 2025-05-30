using Maraudr.Associations.Application.Dtos;
using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Interfaces;

namespace Maraudr.Associations.Application.UseCases.Query;

public interface ISearchAssociationsByNameHandler
{
    Task<List<AssociationDto>> HandleAsync(string name);
}

public class SearchAssociationsByName(IAssociations associations) : ISearchAssociationsByNameHandler
{
    public async Task<List<AssociationDto>> HandleAsync(string name)
    {
        var result = await associations.SearchAssociationsByName(name);
        return result.Select(a => new AssociationDto(
            a.Id,
            a.Name,
            new AddressDto(a.Address.Street, a.Address.City, a.Address.PostalCode)
        )).ToList();
    }
}
