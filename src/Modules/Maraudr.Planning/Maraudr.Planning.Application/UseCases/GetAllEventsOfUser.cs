using Maraudr.Planning.Domain.Entities;
using Maraudr.Planning.Domain.Interfaces;

namespace Maraudr.Planning.Application.UseCases;


public interface IGetAllEventsOfUserHandler
{ 
    Task<IEnumerable<Event>> HandleAsync(Guid userId);
}
public class GetAllEventsOfUserHandler(IPlanningRepository repository):IGetAllEventsOfUserHandler
{
    public async Task<IEnumerable<Event>> HandleAsync(Guid userId)
    {
       var events =  await repository.GetAllUserEventsAsync(userId);
       return events;
    }
}