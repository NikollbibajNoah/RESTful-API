using Microsoft.Extensions.Caching.Memory;
using RESTful.Service.Interface;

namespace RESTful.Service.Implementation;

public class CachingService : ICachingService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingService> _logger;

    public CachingService(IMemoryCache cache, ILogger<CachingService> logger)
    {
        _cache = cache;
        _logger = logger;
    }


    public Task<T?> GetAsync<T>(string key)
    {
        try
        {
            if (_cache.TryGetValue<T>(key, out var cached))
            {
                _logger.LogInformation("[CACHE HIT] Key: {CacheKey}", key);
                return Task.FromResult(cached);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache lookup failed for key {CacheKey}", key);
        }

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null)
    {
        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(absoluteExpiration ?? TimeSpan.FromMinutes(2))
            .SetSlidingExpiration(slidingExpiration ?? TimeSpan.FromMinutes(1))
            .SetSize(1);

        _cache.Set(key, value, options);
        _logger.LogInformation("[CACHE SET] Key: {CacheKey}", key);
        
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        _logger.LogInformation("[CACHE REMOVE] Key: {CacheKey}", key);
        
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        _logger.LogInformation("[CACHE REMOVE PATTERN] Pattern: {Pattern}", pattern);
        return Task.CompletedTask;
    }
}