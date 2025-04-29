namespace Maraudr.Stock.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<StockContext>(options => options.UseInMemoryDatabase("StockTestDb"));
        services.AddScoped<IStockRepository, StockRepository>();
    }
}
