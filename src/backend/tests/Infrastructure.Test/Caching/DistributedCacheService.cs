namespace Infrastructure.Test.Caching;

using CodeMatrix.Mepd.Infrastructure.Common.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

public class DistributedCacheService : CacheService<CodeMatrix.Mepd.Infrastructure.Caching.DistributedCacheService>
{
    protected override CodeMatrix.Mepd.Infrastructure.Caching.DistributedCacheService CreateCacheService() =>
        new(new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions())),
            new NewtonSoftService(),
            NullLogger<CodeMatrix.Mepd.Infrastructure.Caching.DistributedCacheService>.Instance);
}
