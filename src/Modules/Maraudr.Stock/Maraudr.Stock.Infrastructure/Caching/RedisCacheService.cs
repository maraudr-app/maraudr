using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Maraudr.Stock.Infrastructure.Caching;

public class RedisCacheService(IDistributedCache cache) : IRedisCacheService
{
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await cache.GetStringAsync(key);
        return value is null ? default : JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(5)
        };
        var json = JsonSerializer.Serialize(value);
        await cache.SetStringAsync(key, json, options);
    }

    public Task RemoveAsync(string key) => cache.RemoveAsync(key);
}
