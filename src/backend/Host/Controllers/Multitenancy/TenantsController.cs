using CodeMatrix.Mepd.Application.Multitenancy;

namespace CodeMatrix.Mepd.Host.Controllers.Multitenancy;

/// <summary>
/// Tenants controller
/// </summary>
public class TenantsController : VersionNeutralApiController
{
    /// <summary>
    /// Get tenant details
    /// </summary>
    [HttpGet("{tenantId}")]
    [MustHavePermission(RootPermissions.Tenants.View)]
    [OpenApiOperation("Get Tenant Details.", "")]
    public Task<TenantDto> GetAsync(string tenantId)
    {
        return Mediator.Send(new GetTenantRequest(tenantId));
    }

    /// <summary>
    /// Get all the available tenants
    /// </summary>
    [HttpGet]
    [MustHavePermission(RootPermissions.Tenants.ListAll)]
    [OpenApiOperation("Get all the available Tenants.", "")]
    public  Task<List<TenantDto>> GetAllAsync()
    {
        return Mediator.Send(new GetAllTenantsRequest());
    }
    
    /// <summary>
    /// Create a new tenant
    /// </summary>
    [HttpPost]
    [MustHavePermission(RootPermissions.Tenants.Create)]
    [OpenApiOperation("Create a new Tenant.", "")]
    public Task<string> CreateAsync(CreateTenantRequest request)
    {
        return Mediator.Send(request);
    }

    /// <summary>
    /// Upgrade subscription of tenant
    /// </summary>
    [HttpPost("upgrade")]
    [MustHavePermission(RootPermissions.Tenants.UpgradeSubscription)]
    [ApiConventionMethod(typeof(MepdApiConventions), nameof(MepdApiConventions.Register))]
    public  Task<string> UpgradeSubscriptionAsync(UpgradeSubscriptionRequest request)
    {
        return Mediator.Send(request);
    }

    /// <summary>
    /// Deactivate tenant
    /// </summary>
    [HttpPost("{tenantId}/deactivate")]
    [MustHavePermission(RootPermissions.Tenants.Update)]
    [OpenApiOperation("Deactivate Tenant.", "")]
    [ApiConventionMethod(typeof(MepdApiConventions), nameof(MepdApiConventions.Register))]
    public Task<string> DeactivateTenantAsync(string tenantId)
    {
        return Mediator.Send(new DeactivateTenantRequest(tenantId));
    }

    /// <summary>
    /// Activate tenant
    /// </summary>
    [HttpPost("{tenantId}/activate")]
    [MustHavePermission(RootPermissions.Tenants.Update)]
    [OpenApiOperation("Activate Tenant.", "")]
    [ApiConventionMethod(typeof(MepdApiConventions), nameof(MepdApiConventions.Register))]
    public Task<string> ActivateTenantAsync(string tenantId)
    {
        return Mediator.Send(new ActivateTenantRequest(tenantId));
    }
}