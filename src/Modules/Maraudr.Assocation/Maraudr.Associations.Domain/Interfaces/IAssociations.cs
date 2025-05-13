namespace Maraudr.Associations.Domain.Entities;

public interface IAssociations
{
    Task<Association> RegisterAssociation(Association association);
    Task UnregisterAssociation(Guid id);
    Task<Association?> GetAssociation(Guid id);
    Task<Association?> UpdateAssociation(Association association);
}