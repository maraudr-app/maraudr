using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Interfaces;

namespace Maraudr.Associations.Application.UseCases.Query;

public interface ISearchAssociationsByCityHandler
{
    Task<List<Association>> HandleAsync(string city);
}

public class SearchAssociationsByCity(IAssociations associations) : ISearchAssociationsByCityHandler
{
    public async Task<List<Association>> HandleAsync(string city)
    {
        return await associations.SearchAssociationsByCity(city);
    }
}

