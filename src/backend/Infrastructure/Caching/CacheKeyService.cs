using CodeMatrix.Mepd.Application.Common.Caching;
using Finbuckle.MultiTenant;

namespace CodeMatrix.Mepd.Infrastructure.Caching;

public class CacheKeyService: ICacheKeyService
{
    private readonly ITenantInfo? _currentTenant;

    public CacheKeyService(ITenantInfo currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public string GetCacheKey(string name, object id, bool includeTenantId = true)
    {
        string tenantId = includeTenantId
            ? _currentTenant?.Id ?? throw new InvalidOperationException("GetCacheKey: include includeTenantId set to true and no TenantInfo avilable.")
            : "gneric";

        return $"{tenantId}-{name}-{id}";
    }
}
