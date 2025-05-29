using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Interfaces;

namespace Maraudr.Associations.Application.UseCases.Query;

public interface ISearchAssociationsByNameHandler
{
    Task<List<Association>> HandleAsync(string name);
}

public class SearchAssociationsByName(IAssociations associations) : ISearchAssociationsByNameHandler
{
    public async Task<List<Association>> HandleAsync(string name)
    {
        return await associations.SearchAssociationsByName(name);
    }
}
