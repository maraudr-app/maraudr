using Maraudr.Planning.Domain.Interfaces;
using Maraudr.Planning.Infrastructure;
using Maraudr.Planning.Infrastructure.Repositories;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Maraudr.Planning.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<PlanningContext>(options =>
            options.UseNpgsql(connectionString));


        services.AddScoped<IPlanningRepository, PlanningRepository>();
        services.AddScoped<IAssociationRepository, AssociationRepository>();
        services.AddScoped<IEmailingRepository, EmailingRepository>();


    }
}
