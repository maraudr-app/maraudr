using Maraudr.Associations.Domain;
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
    public async Task<Guid> HandleAsync(CreateAssociationCommand command)
    {
        AssociationCreator factory = !string.IsNullOrWhiteSpace(command.Siret)
            ? new AssociationWithSiretCreator(command.Name, command.City, command.Country, new SiretNumber(command.Siret))
            : new BasicAssociationCreator(command.Name, command.City, command.Country);

        var association = factory.CreateAssociation();
        
        var result = await associations.RegisterAssociation(association);
        
        return result.Id;
    }
}