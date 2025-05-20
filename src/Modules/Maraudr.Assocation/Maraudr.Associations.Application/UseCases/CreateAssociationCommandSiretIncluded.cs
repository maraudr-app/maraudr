using Maraudr.Associations.Domain;
using Maraudr.Associations.Domain.Entities;

namespace Maraudr.Associations.Application.UseCases;

public interface ICreateAssociationHandlerSiretIncluded
{
    Task <Guid> HandleAsync(CreateAssociationCommandSiretIncluded association);
}

public record CreateAssociationCommandSiretIncluded(string Name, string City, string Country, string Siret);

public class CreateAssociationSiretIncluded(IAssociations associations) : ICreateAssociationHandlerSiretIncluded
{
    public async Task<Guid> HandleAsync(CreateAssociationCommandSiretIncluded association)
    {
        var factory = new AssociationWithSiretCreator(association.Name, association.City, association.Country, association.Siret);
        var asso = factory.CreateAssociation();
        
        var result = await associations.RegisterAssociation(asso);
        
        return result.Id;
    }
}