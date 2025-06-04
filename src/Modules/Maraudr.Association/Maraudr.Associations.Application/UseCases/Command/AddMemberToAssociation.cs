using Maraudr.Associations.Domain.Interfaces;

namespace Maraudr.Associations.Application.UseCases.Command;

public interface IAddMemberToAssociationHandler
{
    Task HandleAsync(Guid userId, Guid associationId);
}

public class AddMemberToAssociation(IAssociations associations) : IAddMemberToAssociationHandler
{
    public async Task HandleAsync(Guid userId, Guid associationId)
    {
        await associations.AddUserToAssociationAsync(associationId, userId);
    }
}
