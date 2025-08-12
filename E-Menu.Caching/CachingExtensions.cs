using E_Menu.Caching.Caches;
using E_Menu.Caching.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace E_Menu.Caching;

public static class CachingExtensions
{
    public static IServiceCollection AddCachingServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, InMemoryCacheService>();
        return services;
    }
}
