using Microsoft.Extensions.Configuration;

namespace Maraudr.Stock.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddHttpClient<IOpenFoodFactRepository, OpenFoodFactRepository>();
        
        var connectionString = configuration.GetConnectionString("Stock");

        services.AddDbContext<StockContext>(options =>
            options.UseNpgsql(connectionString));
    }
}
