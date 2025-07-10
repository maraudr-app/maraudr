using Maraudr.Geo.Domain.Interfaces;
using Maraudr.Geo.Infrastructure;

namespace Maraudr.Geo.Application.UseCases;

public interface IToogleGeoStatusHandler
{
    Task HandleAsync(Guid id);
}

public class ToogleGeoStatusHandler(IGeoRepository repository) : IToogleGeoStatusHandler
{
    public async Task HandleAsync(Guid id)
    {
        await repository.ToggleGeoStatusAsync(id);
    }
}