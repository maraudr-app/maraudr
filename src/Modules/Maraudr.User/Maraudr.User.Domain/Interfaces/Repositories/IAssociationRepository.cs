using Maraudr.User.Domain.Entities.Users;

namespace Maraudr.User.Domain.Interfaces.Repositories;

public interface IAssociationRepository
{
    public  Task<bool> AssociationExists(Guid id);
    
    public  Task<bool> IsUserMemberOfAssociationAsync(Guid userId, Guid associationId);


    public Task<string> GetAssociationName(Guid associationId);


}