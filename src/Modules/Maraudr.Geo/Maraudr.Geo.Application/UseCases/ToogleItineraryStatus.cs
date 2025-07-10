using Maraudr.Geo.Domain.Interfaces;
using Maraudr.Geo.Infrastructure;

namespace Maraudr.Geo.Application.UseCases;

public interface IToogleItineraryStatusHandler
{
    Task HandleAsync(Guid id);
}

public class ToogleItineraryStatusHandler(IGeoRepository repository) : IToogleItineraryStatusHandler
{
    public async Task HandleAsync(Guid id)
    {
        await repository.ToggleItineraryStatusAsync(id);
    }
}