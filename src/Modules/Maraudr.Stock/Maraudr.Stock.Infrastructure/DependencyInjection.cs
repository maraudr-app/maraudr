using Maraudr.Stock.Infrastructure.Caching;
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
        
        services.AddStackExchangeRedisCache(options =>
        {
            var redisHost = configuration["REDIS_HOST"];
            var redisPassword = configuration["REDIS_PASSWORD"];
    
            options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
            {
                ConnectTimeout = 10000,
                SyncTimeout = 5000,
                AsyncTimeout = 10000,
                ConnectRetry = 3,
                KeepAlive = 180,
                AbortOnConnectFail = false,
                EndPoints = { redisHost },
                Ssl = true,
                Password = redisPassword
            };
        });
        
        services.AddSingleton<IRedisCacheService, RedisCacheService>();
    }
}
