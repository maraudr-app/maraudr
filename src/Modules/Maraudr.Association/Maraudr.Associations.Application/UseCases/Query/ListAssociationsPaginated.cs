using Maraudr.Associations.Application.Dtos;
using Maraudr.Associations.Domain.Interfaces;

namespace Maraudr.Associations.Application.UseCases.Query;

public interface IListAssociationsPaginatedHandler
{
    Task<List<AssociationDto>> HandleAsync(int page, int pageSize);
}

public class ListAssociationsPaginated(IAssociations repository) : IListAssociationsPaginatedHandler
{
    public async Task<List<AssociationDto>> HandleAsync(int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        var associations = await repository.ListPaginated(skip, pageSize);

        return associations.Select(a => new AssociationDto(
            a.Id,
            a.Name,
            new AddressDto(a.Address.Street, a.Address.City, a.Address.PostalCode)
        )).ToList();
    }
}
