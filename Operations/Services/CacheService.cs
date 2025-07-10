// CacheService.cs - Services klasörüne ekleyin

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using CityHotelGarage.Business.Operations.Interfaces;

namespace CityHotelGarage.Business.Operations.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public T? Get<T>(string key)
    {
        try
        {
            if (_cache.TryGetValue(key, out T? value))
            {
                _logger.LogInformation("Cache HIT: {Key}", key);
                return value;
            }
            
            _logger.LogInformation("Cache MISS: {Key}", key);
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache GET error for key: {Key}", key);
            return default;
        }
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var options = new MemoryCacheEntryOptions();
            
            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration;
            }
            else
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // 30 dakika
            }

            options.Priority = CacheItemPriority.Normal;
            options.Size = 1;
            
            _cache.Set(key, value, options);
            _logger.LogInformation("Cache SET: {Key} (Expires: {Expiration})", key, expiration?.ToString() ?? "30m");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache SET error for key: {Key}", key);
        }
    }

    public void Remove(string key)
    {
        try
        {
            _cache.Remove(key);
            _logger.LogInformation("Cache REMOVE: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache REMOVE error for key: {Key}", key);
        }
    }

    public void RemoveByPattern(string pattern)
    {
        try
        {
            // Bilinen cache key'leri manuel temizle
            if (pattern.Contains("cities"))
            {
                Remove("cities:all");
                _logger.LogInformation("🧹 Cache pattern REMOVE: cities:*");
            }
            else if (pattern.Contains("hotels"))
            {
                Remove("hotels:all");
                _logger.LogInformation("🧹 Cache pattern REMOVE: hotels:*");
            }
            
            _logger.LogInformation("🧹 Cache pattern REMOVE completed: {Pattern}", pattern);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache pattern REMOVE error for pattern: {Pattern}", pattern);
        }
    }
}