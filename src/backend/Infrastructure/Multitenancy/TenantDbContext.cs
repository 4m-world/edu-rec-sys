using CodeMatrix.Mepd.Infrastructure.Persistence.Configurations;
using Finbuckle.MultiTenant.Stores;
using Microsoft.EntityFrameworkCore;

namespace CodeMatrix.Mepd.Infrastructure.Multitenancy;

public class TenantDbContext : EFCoreStoreDbContext<MepdTenantInfo>
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MepdTenantInfo>().ToTable("Tenants", SchemaNames.MultiTenancy);
    }

}