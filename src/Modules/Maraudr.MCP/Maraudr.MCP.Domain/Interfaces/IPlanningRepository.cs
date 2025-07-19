namespace Maraudr.MCP.Domain.Interfaces;
public record EventDto(
    Guid Id,
    Guid PlanningId,
    Guid OrganizerdId,
    List<Guid> ParticipantsIds,
    DateTime BeginningDate,
    DateTime EndDate,
    string Title,
    string Description,
    string Location,
    string Status
);
public interface IPlanningRepository
{
    public Task<IEnumerable<EventDto>> GetAllAssociationEventsAsync(Guid associationId,string jwt);

    public Task<IEnumerable<EventDto>> GetAllMyEventsAsync(string jwt);

}