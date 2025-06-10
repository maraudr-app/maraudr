using Maraudr.Geo.Application.Dtos;
using Maraudr.Geo.Domain.Entities;
using Maraudr.Geo.Domain.Interfaces;

namespace Maraudr.Geo.Application.UseCases;

public interface ICreateGeoDataForAnAssociation
{
    Task<GeoDataResponse?> HandleAsync(CreateGeoDataRequest createGeoDataRequest);
}

public class CreateGeoDataForAnAssociation(IGeoRepository repository) : ICreateGeoDataForAnAssociation
{

    public async Task<GeoDataResponse?> HandleAsync(CreateGeoDataRequest request)
    {
        var store = await repository.GetGeoStoreByAssociationAsync(request.AssociationId);
        if (store is null) return null;

        var data = new GeoData
        {
            Id = Guid.NewGuid(),
            GeoStoreId = store.Id,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Notes = request.Notes,
            ObservedAt = DateTime.UtcNow
        };

        await repository.AddEventAsync(data);

        return new GeoDataResponse(data.Id, data.GeoStoreId, data.Latitude, data.Longitude, data.Notes, data.ObservedAt);
    }
}