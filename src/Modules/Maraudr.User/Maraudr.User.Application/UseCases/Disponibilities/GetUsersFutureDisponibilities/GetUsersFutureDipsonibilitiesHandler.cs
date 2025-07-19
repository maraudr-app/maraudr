using Maraudr.User.Domain.Interfaces.Repositories;
using Maraudr.User.Domain.ValueObjects.Users;

namespace Application.UseCases.Disponibilities.GetUsersFutureDisponibilities;

public class GetUsersFutureDipsonibilitiesHandler(IUserRepository repository):IGetUsersFutureDipsonibilitiesHandler
{
    public async Task<IEnumerable<Disponibility>> HandleAsync(Guid userId, Guid associationId)
    {
        var user = await repository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("USer not found");
        }

        return user.Disponibilities.Where(disponibility => disponibility.AssociationId == associationId && disponibility.Start>DateTime.Now);
    }
}