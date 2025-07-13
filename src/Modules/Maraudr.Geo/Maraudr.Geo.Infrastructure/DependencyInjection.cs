using Maraudr.Geo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maraudr.Geo.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IGeoRepository, GeoRepository>();
        
        // Utiliser "Default" pour correspondre à appsettings.json
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<GeoContext>(options =>
            options.UseNpgsql(connectionString, o => o.UseNetTopologySuite()));
    }
}