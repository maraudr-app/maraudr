using Maraudr.User.Domain.ValueObjects.Users;

namespace Application.UseCases.Disponibilities.GetUsersFutureDisponibilities;

public interface IGetUsersFutureDipsonibilitiesHandler
{
    public Task<IEnumerable<Disponibility>> HandleAsync(Guid userId, Guid associationId);

}