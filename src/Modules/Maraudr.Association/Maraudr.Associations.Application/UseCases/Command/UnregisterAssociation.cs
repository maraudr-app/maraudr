using Maraudr.Associations.Domain.Interfaces;

namespace Maraudr.Associations.Application.UseCases.Command;

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