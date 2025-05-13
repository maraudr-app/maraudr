using Maraudr.Associations.Domain.Entities;

namespace Maraudr.Associations.Application.UseCases;

public interface IGetAssociationHandler
{
    public Task<Association?> HandleAsync(Guid id);
}

public class GetAssociation(IAssociations associations) : IGetAssociationHandler
{
    public async Task<Association?> HandleAsync(Guid id)
    {
        var result = await associations.GetAssociation(id);
        return result ?? null;
    }
}