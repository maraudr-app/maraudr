using Maraudr.Geo.Domain.Entities;
using Maraudr.Geo.Domain.Interfaces;

namespace Maraudr.Geo.Application.UseCases;

public interface IGetGeoStoreInfoForAnAssociation
{
    Task<GeoStore?> HandleAsync(Guid associationId);
}

public class GetGeoStoreInfoForAnAssociation(IGeoRepository repository) : IGetGeoStoreInfoForAnAssociation
{
    public async Task<GeoStore?> HandleAsync(Guid associationId)
    {
        return await repository.GetGeoStoreByAssociationAsync(associationId);
    }
}