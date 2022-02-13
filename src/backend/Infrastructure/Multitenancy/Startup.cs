using CodeMatrix.Mepd.Application.Multitenancy;
using CodeMatrix.Mepd.Infrastructure.Persistence;
using CodeMatrix.Mepd.Shared.Authorization;
using CodeMatrix.Mepd.Shared.Multitenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace CodeMatrix.Mepd.Infrastructure.Multitenancy;

internal static class Startup
{
    internal static IServiceCollection AddMultitenancy(this IServiceCollection services, IConfiguration config)
    {
        // TODO: We should probably add specific dbprovider/connectionstring setting for the tenantDb with a fallback to the main databasesettings
        var databaseSettings = config.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        string rootConnectionString = databaseSettings.ConnectionString;
        if (string.IsNullOrEmpty(rootConnectionString)) throw new InvalidOperationException("DB ConnectionString is not configured.");
        string dbProvider = databaseSettings.DBProvider;
        if (string.IsNullOrEmpty(dbProvider)) throw new InvalidOperationException("DB Provider is not configured.");

        return services
            .AddDbContext<TenantDbContext>(m => m.UseDatabase(dbProvider, rootConnectionString))
            .AddMultiTenant<MepdTenantInfo>()
                .WithClaimStrategy(MepdClaims.Tenant)
                .WithHeaderStrategy(MultitenancyConstants.TenantIdName)
                .WithQueryStringStrategy(MultitenancyConstants.TenantIdName)
                .WithEFCoreStore<TenantDbContext, MepdTenantInfo>()
                .Services
            .AddScoped<ITenantService, TenantService>();
    }

    private static FinbuckleMultiTenantBuilder<MepdTenantInfo> WithQueryStringStrategy(
        this FinbuckleMultiTenantBuilder<MepdTenantInfo> builder, 
        string queryStringKey) =>
        builder.WithDelegateStrategy(context =>
        {
            if (context is not HttpContext httpContext)
            {
                return Task.FromResult((string)null);
            }
            
            httpContext.Request.Query.TryGetValue(queryStringKey, out StringValues tenantIdParam);
            return Task.FromResult(tenantIdParam.ToString());
        });


    internal static IApplicationBuilder UseCurrentTenant(this IApplicationBuilder app) =>
        app.UseMultiTenant();
}