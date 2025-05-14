using Maraudr.Associations.Domain.Entities;

namespace Maraudr.Associations.Application.UseCases;

public interface IUnregisterAssociation
{
    Task HandleAsync(Guid id);
}

public class UnregisterAssociation(IAssociations associations) : IUnregisterAssociation
{
    public async Task HandleAsync(Guid id)
    {
        await associations.UnregisterAssociation(id);
    }
}