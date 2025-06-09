using Maraudr.User.Domain.ValueObjects.Users;

namespace Application.UseCases.Disponibilities.GetUsersDipsonibilities;

public interface IGetUsersDipsonibilitiesHandler
{
    public Task<IEnumerable<Disponibility>> HandleAsync(Guid userId, Guid associationId);
}