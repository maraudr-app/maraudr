using Maraudr.Associations.Domain;
using Maraudr.Associations.Domain.Entities;

namespace Maraudr.Associations.Application.UseCases;

public interface ICreateAssociationHandler
{
    Task<Guid> HandleAsync(CreateAssociationCommandBasic commandCommandBasic);
}

public class CreateAssociationBasic(IAssociations associations) : ICreateAssociationHandler
{
    public async Task<Guid> HandleAsync(CreateAssociationCommandBasic commandCommandBasic)
    {
        AssociationCreator factory = new BasicAssociationCreator(commandCommandBasic.Name, commandCommandBasic.City, commandCommandBasic.Country);
        var association = factory.CreateAssociation();
        
        var result = await associations.RegisterAssociation(association);
        
        return result.Id;
    }
}