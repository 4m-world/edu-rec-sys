namespace CodeMatrix.Mepd.Application.Common.Caching;

public interface ICacheService
{
    T Get<T>(string key);
    Task<T> GetAsync<T>(string key, CancellationToken token = default);

    void Refresh(string key);
    Task RefreshAsync(string key, CancellationToken token = default);

    void Remove(string key);
    Task RemoveAsync(string key, CancellationToken token = default);

    void Set<T>(string key, T value, TimeSpan? slidingExpiration = null);
    Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default);
}

public interface ICacheKeyService : IScopedService
{
    public string GetCacheKey(string name, object id, bool includeTenantId = true);
}

public static class CacheKeyServiceExtensions
{
    public static string GetCacheKey<TEntity>(this ICacheKeyService cacheKeyService, object id, bool includeTenantId = true)
        where TEntity: IEntity
    {
        return cacheKeyService.GetCacheKey(typeof(TEntity).Name, id, includeTenantId);
    }
}