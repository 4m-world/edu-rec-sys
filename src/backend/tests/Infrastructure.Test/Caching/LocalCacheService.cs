namespace Infrastructure.Test.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

public class LocalCacheService : CacheService<CodeMatrix.Mepd.Infrastructure.Caching.LocalCacheService>
{
    protected override CodeMatrix.Mepd.Infrastructure.Caching.LocalCacheService CreateCacheService() =>
        new(new MemoryCache(new MemoryCacheOptions()),
            NullLogger<CodeMatrix.Mepd.Infrastructure.Caching.LocalCacheService>.Instance);
}