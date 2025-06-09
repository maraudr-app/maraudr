using Maraudr.Geo.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Maraudr.Geo.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateGeoStoreForAnAssociation, CreateGeoStoreForAnAssociation>();
    }
}