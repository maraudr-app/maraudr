using Maraudr.Geo.Application.Dtos;
using Maraudr.Geo.Domain.Interfaces;

namespace Maraudr.Geo.Application.UseCases;

public interface IGetAllGeoDataForAnAssociation
{
    Task<List<GeoDataResponse>> HandleAsync(Guid associationId, int days);
}

public class GetAllGeoDataForAnAssociation(IGeoRepository repository) : IGetAllGeoDataForAnAssociation
{
    public async Task<List<GeoDataResponse>> HandleAsync(Guid associationId, int days)
    {
        var from = DateTime.UtcNow.AddDays(-days);
        var data = await repository.GetEventsAsync(associationId, from);

        var response = data.Select(
            x => new GeoDataResponse(
                x.Id, x.GeoStoreId,
                x.Latitude,
                x.Longitude,
                x.Notes,
                x.ObservedAt))
            .ToList();
        
        return response;
    }
}