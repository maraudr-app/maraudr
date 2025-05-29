using Maraudr.Associations.Domain.Entities;
using Maraudr.Associations.Domain.Interfaces;
using Maraudr.Associations.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Maraudr.Associations.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AssociationsContext>(options => options.UseInMemoryDatabase("AssociationsTestDb"));
        services.AddScoped<IAssociations, AssocationsRepository>();
    }
}
