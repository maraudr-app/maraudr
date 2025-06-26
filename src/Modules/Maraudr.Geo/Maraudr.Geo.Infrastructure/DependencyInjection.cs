using Maraudr.Geo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maraudr.Geo.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GeoContext>(
            options => options.UseNpgsql(configuration.GetConnectionString("GeoDb"),         
                o => o.UseNetTopologySuite()
            ));

        services.AddScoped<IGeoRepository, GeoRepository>();
        services.AddScoped<IItineraryRepository, ItineraryRepository>();
    }
}