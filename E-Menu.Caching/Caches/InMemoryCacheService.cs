using E_Menu.Caching.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace E_Menu.Caching.Caches;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _defaultExpiration;

    public InMemoryCacheService(IMemoryCache memoryCache)
    {

        _memoryCache = memoryCache;
        _defaultExpiration = TimeSpan.FromMinutes(30);
    }

    public Task<bool> ExistsAsync(string key)
    {
        return Task.FromResult(_memoryCache.TryGetValue(key, out _));
    }

    public Task<T> GetAsync<T>(string key)
    {
        _memoryCache.TryGetValue(key, out T value);
        return Task.FromResult(value);
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (_memoryCache.TryGetValue(key, out T value))
        {
            return value;
        }
        value = await factory();
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
        };
        _memoryCache.Set(key, value, cacheEntryOptions);
        return value;
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
        };
        _memoryCache.Set(key, value, cacheEntryOptions);
        return Task.CompletedTask;
    }
}