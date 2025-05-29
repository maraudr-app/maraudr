using Maraudr.Associations.Domain.Entities;

namespace Maraudr.Associations.Domain.Interfaces;

public interface IAssociations
{
    Task<Association?> RegisterAssociation(Association? association);
    Task UnregisterAssociation(Guid id);
    Task<Association?> GetAssociation(Guid id);
    Task<Association?> UpdateAssociation(Association association);
    Task<Association?> GetAssociationBySiret(string siret);
    Task<List<Association>> SearchAssociationsByName(string name);
    Task<List<Association>> SearchAssociationsByCity(string city);
}