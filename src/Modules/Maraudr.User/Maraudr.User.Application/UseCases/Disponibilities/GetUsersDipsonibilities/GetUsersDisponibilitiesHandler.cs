using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.ValueObjects.Users;

namespace Application.UseCases.Disponibilities.GetUsersDipsonibilities;

public class GetUsersDisponibilitiesHandler(IUserRepository repository):IGetUsersDisponibilitiesHandler
{
    public async Task<IEnumerable<Disponibility>> HandleAsync(Guid userId, Guid associationId)
    {
        
        var user = await repository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        return user.Disponibilities.Where(disponibility => disponibility.AssociationId == associationId);
        
    }
}