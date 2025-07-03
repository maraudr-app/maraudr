using Maraudr.Geo.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Maraudr.Geo.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateGeoStoreForAnAssociation, CreateGeoStoreForAnAssociation>();
        services.AddScoped<ICreateGeoDataForAnAssociation, CreateGeoDataForAnAssociation>();
        services.AddScoped<IGetAllGeoDataForAnAssociation, GetAllGeoDataForAnAssociation>();
        services.AddScoped<IGetGeoStoreInfoForAnAssociation, GetGeoStoreInfoForAnAssociation>();
        services.AddScoped<IGetGeoRouteHandler, GetGeoRouteHandler>();
        services.AddScoped<ICreateItineraryHandler, CreateItineraryHandler>();
        services.AddScoped<IGetItineraryHandler, GetItineraryHandler>();
    }
}
