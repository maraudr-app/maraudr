using Maraudr.User.Domain.ValueObjects.Users;

namespace Application.UseCases.Disponibilities.GetAllAssociationUsersDipsonibilities;

public interface IGetAllAssociationUsersDipsonibilitiesHandler
{
    public Task<IEnumerable<Disponibility>> HandleAsync(Guid userId, Guid associationId);

}