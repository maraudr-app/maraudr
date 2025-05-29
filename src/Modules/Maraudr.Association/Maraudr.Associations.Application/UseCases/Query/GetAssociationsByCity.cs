using Maraudr.Associations.Application.Dtos;
using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Interfaces;

namespace Maraudr.Associations.Application.UseCases.Query;

public interface ISearchAssociationsByCityHandler
{
    Task<List<AssociationDto>> HandleAsync(string city);
}

public class SearchAssociationsByCity(IAssociations associations) : ISearchAssociationsByCityHandler
{
    public async Task<List<AssociationDto>> HandleAsync(string city)
    {
        var results = await associations.SearchAssociationsByCity(city);
        return results.Select(a => new AssociationDto(
            a.Id,
            a.Name,
            new AddressDto(a.Address.Street, a.Address.City, a.Address.PostalCode)
        )).ToList();
    }
}

