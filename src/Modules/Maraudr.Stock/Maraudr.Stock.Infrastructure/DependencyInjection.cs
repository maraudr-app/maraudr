using Maraudr.Stock.Infrastructure.Caching;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Maraudr.Stock.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddHttpClient<IOpenFoodFactRepository, OpenFoodFactRepository>();
        
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<StockContext>(options =>
            options.UseNpgsql(connectionString));
        
        var redisHost = configuration["REDIS_HOST"];
        var redisPassword = configuration["REDIS_PASSWORD"];

        var options = new ConfigurationOptions
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

        var multiplexer = ConnectionMultiplexer.Connect(options);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        services.AddStackExchangeRedisCache(opt => opt.ConfigurationOptions = options);
        
        services.AddSingleton<IRedisCacheService, RedisCacheService>();
    }
}