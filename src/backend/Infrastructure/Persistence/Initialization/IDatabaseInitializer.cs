using CodeMatrix.Mepd.Infrastructure.Multitenancy;

namespace CodeMatrix.Mepd.Infrastructure.Persistence.Initialization;

internal interface IDatabaseInitializer
{
    Task InitializeDatabasesAsync(CancellationToken cancellationToken);
    Task InitializeApplicationDbForTenantAsync(MepdTenantInfo tenant, CancellationToken cancellationToken);
}