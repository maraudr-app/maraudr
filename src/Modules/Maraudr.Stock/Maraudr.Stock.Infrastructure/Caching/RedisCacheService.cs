using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Maraudr.Stock.Infrastructure.Caching;

public class RedisCacheService(
    IDistributedCache cache,
    IConnectionMultiplexer redis) : IRedisCacheService
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

        var associationKey = GetAssociationKeyFrom(key);
        var indexKey = $"cache-index:{associationKey}";

        var existing = await GetAsync<List<string>>(indexKey) ?? new List<string>();
        if (!existing.Contains(key))
        {
            existing.Add(key);
            await SetAsync(indexKey, existing);
        }
    }
    
    private string GetAssociationKeyFrom(string key)
    {
        
        var parts = key.Split(':', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2 || parts[0] != "items")
            throw new ArgumentException($"Clé de cache invalide ou non prise en charge : {key}");

        return $"items:{parts[1]}"; 
    }


    public Task RemoveAsync(string key) => cache.RemoveAsync(key);

    public async Task RemoveAllForAssociationAsync(Guid associationId)
    {
        var indexKey = $"cache-index:items:{associationId}";
        var keys = await GetAsync<List<string>>(indexKey);

        if (keys is not null)
        {
            foreach (var key in keys)
            {
                await RemoveAsync(key);
            }

            await RemoveAsync(indexKey); // Nettoyage de l’index
        }
    }
}