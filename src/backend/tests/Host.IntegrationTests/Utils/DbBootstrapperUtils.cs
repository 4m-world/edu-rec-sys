using CodeMatrix.Mepd.Domain.Multitenancy;
using CodeMatrix.Mepd.Infrastructure.Identity;
using CodeMatrix.Mepd.Infrastructure.Multitenancy;
using CodeMatrix.Mepd.Infrastructure.Persistence.Contexts;
using CodeMatrix.Mepd.Shared.Multitenancy;
using Host.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Linq;

namespace Host.IntegrationTests.Utils;

public static class DbBootstrapperUtils
{
    public static void CreateDbAndSeedDataIfNotExists(IServiceProvider scopedServices)
    {
        var logger = scopedServices
            .GetRequiredService<ILogger<CustomWebApplicationFactory<Program>>>();
        try
        {
            var tenantDbContext = scopedServices.GetRequiredService<TenantDbContext>();

            SeedRootTenant(tenantDbContext);

            var appDbContext = scopedServices.GetRequiredService<ApplicationDbContext>();
            var userManager = scopedServices.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scopedServices.GetRequiredService<RoleManager<ApplicationRole>>();
            SeedTenantDatabase(appDbContext, tenantDbContext, userManager, roleManager);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error creating tables and seed data: {ex.Message}");
        }
    }

    public static void SeedRootTenant(TenantDbContext dbContext)
    {
        if (!dbContext.TenantInfo.Any(t => t.Id == MultitenancyConstants.Root.Id))
        {
            var rootTenant = new MepdTenantInfo(
                MultitenancyConstants.Root.Id,
                MultitenancyConstants.Root.Name,
                 _dbSettings.ConnectionString!,
                MultitenancyConstants.Root.EmailAddress,
                "TenantDbForTesting");
            rootTenant.SetValidity(DateTime.UtcNow.AddYears(1));
            dbContext.TenantInfo.Add(rootTenant);
            dbContext.SaveChanges();
        }
    }
}
