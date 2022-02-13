using CodeMatrix.Mepd.Infrastructure;
using CodeMatrix.Mepd.Infrastructure.Auth;
using CodeMatrix.Mepd.Infrastructure.BackgroundJobs;
using CodeMatrix.Mepd.Infrastructure.Caching;
using CodeMatrix.Mepd.Infrastructure.Common;
using CodeMatrix.Mepd.Infrastructure.Cors;
using CodeMatrix.Mepd.Infrastructure.FileStorage;
using CodeMatrix.Mepd.Infrastructure.Localization;
using CodeMatrix.Mepd.Infrastructure.Mailing;
using CodeMatrix.Mepd.Infrastructure.Mapping;
using CodeMatrix.Mepd.Infrastructure.Middleware;
using CodeMatrix.Mepd.Infrastructure.Monitoring;
using CodeMatrix.Mepd.Infrastructure.Multitenancy;
using CodeMatrix.Mepd.Infrastructure.Notifications;
using CodeMatrix.Mepd.Infrastructure.OpenApi;
using CodeMatrix.Mepd.Infrastructure.Persistence;
using CodeMatrix.Mepd.Infrastructure.Persistence.Initialization;
using CodeMatrix.Mepd.Infrastructure.SecurityHeaders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMatrix.Mepd.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        MapsterSettings.Configure();

        return services
            .AddApiVersioning()
            .AddAuth(config)
            .AddBackgroundJobs(config)
            .AddCaching(config)
            .AddCorsPolicy(config)
            .AddExceptionMiddleware()
            .AddHealthCheck()
            .AddLocalization(config)
            .AddMailing(config)
            .AddMultitenancy(config)
            .AddNotifications(config)
            .AddOpenApiDocumentation(config)
            .AddPersistence(config)
            .AddRequestLogging(config)
            .AddRouting(options => options.LowercaseUrls = true)
            .AddAppMetric(config)
            .AddServices();
    }
    private static IServiceCollection AddApiVersioning(this IServiceCollection services)
    {
        return services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });
    }
    private static IServiceCollection AddHealthCheck(this IServiceCollection services)
    {
        return services.AddHealthChecks().AddCheck<TenantHealthCheck>("Tenant").Services;
    }

    public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>()
            .InitializeDatabasesAsync(cancellationToken);
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder appBuilder, IConfiguration config)
    {
        return appBuilder
            .UseLocalization(config)
            .UseStaticFiles()
            .UseSecurityHeaders(config)
            .UseFileStorage()
            .UseExceptionMiddleware()
            .UseLocalization(config)
            .UseRouting()
            .UseCorsPolicy()
            .UseAuthentication()
            .UseCurrentUser()
            .UseCurrentTenant()
            .UseAuthorization()
            .UseRequestLogging(config)
            .UseHangfireDashboard(config)
            .UseOpenApiDocumentation(config);
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapControllers().RequireAuthorization();
        builder.MapHealthCheck();
        builder.MapNotifications();
        return builder;
    }

    private static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapHealthChecks("/api/health").RequireAuthorization();
}