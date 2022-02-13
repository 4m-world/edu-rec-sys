using CodeMatrix.Mepd.Application.Common.Exceptions;
using CodeMatrix.Mepd.Application.Common.Persistence;
using CodeMatrix.Mepd.Application.Multitenancy;
using CodeMatrix.Mepd.Infrastructure.Persistence.Initialization;
using Finbuckle.MultiTenant;
using Mapster;
using Microsoft.Extensions.Localization;

namespace CodeMatrix.Mepd.Infrastructure.Multitenancy;

/// <summary>
/// Tenant manager
/// </summary>
internal class TenantService : ITenantService
{
    private readonly IMultiTenantStore<MepdTenantInfo> _tenantStore;
    private readonly IAnonymizeConnectionString _csSecurer;
    private readonly IDatabaseInitializer _dbInitializer;
    private readonly IStringLocalizer<TenantService> _localizer;

    public TenantService(
        IMultiTenantStore<MepdTenantInfo> tenantStore,
        IAnonymizeConnectionString csSecurer, 
        IDatabaseInitializer dbInitializer,
        IStringLocalizer<TenantService> localizer)
    {
        _tenantStore = tenantStore;
        _csSecurer = csSecurer;
        _dbInitializer = dbInitializer;
        _localizer = localizer;
    }

    public async Task<List<TenantDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var tenants = (await _tenantStore.GetAllAsync()).Adapt<List<TenantDto>>();

        tenants.ForEach(t => t.ConnectionString = _csSecurer.Anonymize(t.ConnectionString));

        return tenants;
    }

    public async Task<bool> ExistsWithIdAsync(string id) =>
    await _tenantStore.TryGetAsync(id) is not null;

    public async Task<bool> ExistsWithNameAsync(string name) =>
        (await _tenantStore.GetAllAsync()).Any(t => t.Name == name);

    public async Task<TenantDto> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        (await GetTenantInfoAsync(id))
            .Adapt<TenantDto>();

    public async Task<string> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var tenant = new MepdTenantInfo(request.Id, request.Name, request.ConnectionString, request.AdminEmail, request.Issuer);

        await _tenantStore.TryAddAsync(tenant);

        // TODO: run this in a hangfire job? will then have to send mail when it's ready or not
        try
        {
            await _dbInitializer.InitializeApplicationDbForTenantAsync(tenant, cancellationToken);
        }
        catch
        {
            await _tenantStore.TryRemoveAsync(request.Id);
            throw;
        }

        return tenant.Id;
    }

    public async Task<string> UpdateSubscription(string id, DateTime extendedExpiryDate, CancellationToken cancellationToken = default)
    {
        var tenant = await GetTenantInfoAsync(id);

        tenant.SetValidity(extendedExpiryDate);

        await _tenantStore.TryUpdateAsync(tenant);

        return $"Tenant {id}'s Subscription Upgraded. Now Valid till {tenant.ValidUpto}.";
    }

    public async Task<string> DeactivateAsync(string id, CancellationToken cancellationToken = default)
    {
        var tenant = await GetTenantInfoAsync(id);

        if (!tenant.IsActive)
        {
            throw new ConflictException("Tenant is already Deactivated.");
        }

        tenant.Deactivate();

        await _tenantStore.TryUpdateAsync(tenant);

        return $"Tenant {id} is now Deactivated.";
    }

    public async Task<string> ActivateAsync(string id, CancellationToken cancellationToken = default)
    {
        var tenant = await GetTenantInfoAsync(id);

        if (tenant.IsActive)
        {
            throw new ConflictException("Tenant is already Activated.");
        }

        tenant.Activate();

        await _tenantStore.TryUpdateAsync(tenant);

        return $"Tenant {id} is now Activated.";
    }

    private async Task<MepdTenantInfo> GetTenantInfoAsync(string id, CancellationToken cancellationToken = default) =>
        await _tenantStore.TryGetAsync(id)
        ?? throw new NotFoundException(string.Format(_localizer["entity.notfound"], typeof(MepdTenantInfo).Name, id));

}