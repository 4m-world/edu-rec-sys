namespace CodeMatrix.Mepd.Application.Multitenancy;

public interface ITenantService : ITransientService
{
    Task<List<TenantDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsWithIdAsync(string id);
    Task<bool> ExistsWithNameAsync(string name);
    Task<TenantDto> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<string> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken = default);
    Task<string> ActivateAsync(string tenant, CancellationToken cancellationToken = default);
    Task<string> DeactivateAsync(string tenant, CancellationToken cancellationToken = default);
    Task<string> UpdateSubscription(string id, DateTime extendedExpiryDate, CancellationToken cancellationToken = default);
}