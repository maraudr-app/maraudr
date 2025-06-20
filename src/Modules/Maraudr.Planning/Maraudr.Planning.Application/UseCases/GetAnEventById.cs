using Maraudr.Planning.Domain.Entities;
using Maraudr.Planning.Domain.Interfaces;

namespace Maraudr.Planning.Application.UseCases;

public interface IGetAnEventByIdHandler
{
    public Task<Event?> HandleAsync(Guid eventId);
}



public class GetAnEventByIdHandler(IPlanningRepository repository) : IGetAnEventByIdHandler
{
    public async Task<Event?> HandleAsync(Guid eventId)
    {
       return await repository.GetEventByIdAsync(eventId);
    }
}