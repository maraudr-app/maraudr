using Maraudr.Geo.Domain.Entities;
using Maraudr.Geo.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maraudr.Geo.Application.UseCases;

public interface IGetItineraryHandler
{
    Task<Itinerary?> GetByIdAsync(Guid id);
    Task<List<Itinerary>> GetByAssociationIdAsync(Guid associationId);
}

public class GetItineraryHandler(IItineraryRepository repo) : IGetItineraryHandler
{
    public async Task<Itinerary?> GetByIdAsync(Guid id) => await repo.GetByIdAsync(id);

    public async Task<List<Itinerary>> GetByAssociationIdAsync(Guid associationId)
        => await repo.GetByAssociationIdAsync(associationId);
}
