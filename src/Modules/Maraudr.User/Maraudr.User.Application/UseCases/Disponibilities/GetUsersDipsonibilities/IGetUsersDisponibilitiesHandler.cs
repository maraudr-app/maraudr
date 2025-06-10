using Maraudr.User.Domain.ValueObjects.Users;

namespace Application.UseCases.Disponibilities.GetUsersDipsonibilities;

public interface IGetUsersDisponibilitiesHandler
{
    public Task<IEnumerable<Disponibility>> HandleAsync(Guid userId, Guid associationId);
}