using Maraudr.Associations.Domain.Interfaces;

namespace Maraudr.Associations.Application.UseCases.Query;

public interface IGetAssocationsOfUserHandler
{
    Task<IEnumerable<Guid>> HandleAsync(Guid id);
}

public class GetAssociationsOfUser(IAssociations associations) : IGetAssocationsOfUserHandler
{
    public async Task<IEnumerable<Guid>> HandleAsync(Guid id)
    {
        return await associations.GetAssociationIdsByUserIdAsync(id);
    }
}
