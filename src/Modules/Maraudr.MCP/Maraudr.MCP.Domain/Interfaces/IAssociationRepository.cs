namespace Maraudr.MCP.Domain.Interfaces;
public record AssociationDto(Guid Id, string Name);
public interface IAssociationRepository
{
    public Task<AssociationDto> GetAssociationByName(string name,string jwt);
    Task<IEnumerable<AssociationDto>> GetUserAssociations(string jwt);
}