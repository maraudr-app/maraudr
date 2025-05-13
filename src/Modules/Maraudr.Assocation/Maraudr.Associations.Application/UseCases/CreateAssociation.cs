using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Siret;

namespace Maraudr.Associations.Application.UseCases;

public interface ICreateAssociationHandler
{
    Task<Guid> HandleAsync(CreateAssociationCommand command);
}

public record CreateAssociationCommand(string Name, string City, string Country,  string? Siret);

public class CreateAssociation(IAssociations associations) : ICreateAssociationHandler
{
    public async Task<Guid> HandleAsync(CreateAssociationCommand association)
    {
        var asso = !string.IsNullOrWhiteSpace(association.Siret) ? new Association(association.Name, association.City, association.Country, new SiretNumber(association.Siret)) : new Association(association.Name, association.City, association.Country);
        var result = await associations.RegisterAssociation(asso);
        return result.Id;
    }
}