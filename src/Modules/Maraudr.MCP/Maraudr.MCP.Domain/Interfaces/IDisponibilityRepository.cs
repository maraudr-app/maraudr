namespace Maraudr.MCP.Domain.Interfaces;

public record DisponibilityDto(
 Guid Id,
 Guid UserId,
 DateTime Start,
 DateTime End,
 Guid AssociationId); // Champ obligatoire pour l'association);
public interface IDisponibilityRepository
{
 public Task<IEnumerable<DisponibilityDto>> GetMyDisponibilitiesInAssociation(Guid associationId,string jwt);
 
 public Task<IEnumerable<DisponibilityDto>> GetAllDisponibilitiesInAssociation(Guid associationId,string jwt);

}