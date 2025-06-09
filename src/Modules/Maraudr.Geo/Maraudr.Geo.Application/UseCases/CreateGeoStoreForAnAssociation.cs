using Maraudr.Geo.Application.Dtos;
using Maraudr.Geo.Domain.Entities;
using Maraudr.Geo.Domain.Interfaces;

namespace Maraudr.Geo.Application.UseCases;

public interface ICreateGeoStoreForAnAssociation
{
    Task<GeoStoreResponse> HandleAsync(CreateGeoStoreRequest request);
}

public class CreateGeoStoreForAnAssociation(IGeoRepository repository) : ICreateGeoStoreForAnAssociation
{
    public async Task<GeoStoreResponse> HandleAsync(CreateGeoStoreRequest request)
    {
        var existing = await repository.GetGeoStoreByAssociationAsync(request.AssociationId);
        if (existing is not null)
            throw new Exception("GeoStore already exists for this association");

        var created = await repository.CreateGeoStoreAsync(request.AssociationId);

        return new GeoStoreResponse(created.Id, created.AssociationId);
    }
}