using Maraudr.Associations.Domain.Interfaces;

namespace Maraudr.Associations.Application.UseCases.Query;

public interface IIsUserMemberOfAssociationHandler
{
    public Task<bool> HandleAsync(Guid userId, Guid associationId);
}
public class IsUserMemberOfAssociationHandler(IAssociations repository):IIsUserMemberOfAssociationHandler
{
    public async Task<bool> HandleAsync(Guid userId, Guid associationId)
    {
        var associationIds = await repository.GetAssociationIdsByUserIdAsync(userId);
        return associationIds.Contains(associationId);
    }
}