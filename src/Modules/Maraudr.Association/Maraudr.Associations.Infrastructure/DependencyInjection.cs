using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Interfaces;
using Maraudr.Associations.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maraudr.Associations.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<AssociationsContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IAssociations, AssocationsRepository>();
    }
}