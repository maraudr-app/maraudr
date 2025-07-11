using Maraudr.Geo.Application.Dtos;
using Maraudr.Geo.Domain.Interfaces;
using Maraudr.Geo.Infrastructure.GeoData;

namespace Maraudr.Geo.Application.UseCases;

public interface IGetAllGeoDataForAnAssociation
{
    Task<List<GeoDataResponseWithAddress>> HandleAsync(Guid associationId, int days);
}

public class GetAllGeoDataForAnAssociation(
    IGeoRepository repository,
    IGeoAddressEnricher enricher) : IGetAllGeoDataForAnAssociation
{
    public async Task<List<GeoDataResponseWithAddress>> HandleAsync(Guid associationId, int days)
    {
        var from = DateTime.UtcNow.AddDays(-days);
        var data = await repository.GetEventsAsync(associationId, from);

        var response = new List<GeoDataResponseWithAddress>();

        foreach (var x in data)
        {
            var address = await enricher.GetAddressAsync(x.Latitude, x.Longitude);
            response.Add(new GeoDataResponseWithAddress(
                x.Id,
                x.GeoStoreId,
                x.Latitude,
                x.Longitude,
                x.Notes,
                x.ObservedAt,
                address,
                x.IsActive
            ));
        }

        return response;
    }
}
