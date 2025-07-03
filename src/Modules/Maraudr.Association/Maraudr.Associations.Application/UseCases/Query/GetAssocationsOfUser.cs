using Maraudr.Associations.Application.Dtos;
using Maraudr.Associations.Domain.Interfaces;

namespace Maraudr.Associations.Application.UseCases.Query;

public interface IGetAssocationsOfUserHandler
{
    Task<IEnumerable<AssociationSummaryDto>> HandleAsync(Guid id);
}

public class GetAssociationsOfUser(IAssociations associations) : IGetAssocationsOfUserHandler
{
    public async Task<IEnumerable<AssociationSummaryDto>> HandleAsync(Guid id)
    {
        var asso = await associations.GetAssociationsOfUserAsync(id);

        return asso.Select(a => new AssociationSummaryDto(a.Id, a.Name));
    }
}

