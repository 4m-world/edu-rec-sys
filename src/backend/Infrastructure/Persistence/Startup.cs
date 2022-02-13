using CodeMatrix.Mepd.Application.Common.Persistence;
using CodeMatrix.Mepd.Domain.Common.Contracts;
using CodeMatrix.Mepd.Infrastructure.Common;
using CodeMatrix.Mepd.Infrastructure.Multitenancy;
using CodeMatrix.Mepd.Infrastructure.Persistence.ConnectionString;
using CodeMatrix.Mepd.Infrastructure.Persistence.Contexts;
using CodeMatrix.Mepd.Infrastructure.Persistence.Initialization;
using CodeMatrix.Mepd.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Text;

namespace CodeMatrix.Mepd.Infrastructure.Persistence;

internal static class Startup
{
    private static readonly ILogger _logger = Log.ForContext(typeof(Startup));

    internal static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        // TODO: there must be a cleaner way to do IOptions validation...
        var databaseSettings = config.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        string rootConnectionString = databaseSettings.ConnectionString;
        if (string.IsNullOrEmpty(rootConnectionString)) throw new InvalidOperationException("DB ConnectionString is not configured.");
        string dbProvider = databaseSettings.DBProvider;
        if (string.IsNullOrEmpty(dbProvider)) throw new InvalidOperationException("DB Provider is not configured.");
        _logger.Information($"Current DB Provider : {dbProvider}");

        return services
             .Configure<DatabaseSettings>(config.GetSection(nameof(DatabaseSettings)))
             .AddDbContext<ApplicationDbContext>(m => m.UseDatabase(dbProvider, rootConnectionString))
             .AddTransient<IDatabaseInitializer, DatabaseInitializer>()
             .AddTransient<ApplicationDbInitializer>()
             .AddTransient<ApplicationDbSeeder>()
             .AddServices(typeof(ICustomSeeder), ServiceLifetime.Transient)
             .AddTransient<CustomSeederRunner>()
             
             .AddTransient<IAnonymizeConnectionString, AnonymizeConnectionString>()
             .AddTransient<IConnectionStringValidator, ConnectionStringValidator>()
             .AddSingleton(typeof(IEncryptionProvider), new AesProvider(Encoding.UTF8.GetBytes(databaseSettings.EncryptionKey)))

             // Add Repositories
             .AddRepositories();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Add Repositories
        services.AddScoped(typeof(IRepository<>), typeof(ApplicationDbRepository<>));

        // Add ReadRepositories
        foreach (var aggregateRootType in
            typeof(IAggregateRoot).Assembly.GetExportedTypes()
                .Where(t => typeof(IAggregateRoot).IsAssignableFrom(t) && t.IsClass)
                .ToList())
        {
            services.AddScoped(typeof(IReadRepository<>).MakeGenericType(aggregateRootType), sp =>
                sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)));

            // Decorate the repositories with EventAddingRepositoryDecorators and expose them as IRepositoryWithEvents.
            services.AddScoped(typeof(IRepositoryWithEvents<>).MakeGenericType(aggregateRootType), sp =>
                Activator.CreateInstance(
                    typeof(EventAddingRepositoryDecorator<>).MakeGenericType(aggregateRootType),
                    sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)))
                ?? throw new InvalidOperationException($"Couldn't create EventAddingRepositoryDecorator for aggregateRootType {aggregateRootType.Name}"));
        }

        return services;
    }

    internal static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider, string connectionString) =>
        dbProvider.ToLowerInvariant() switch
        {
            DbProviderKeys.SqlServer =>
                builder.UseSqlServer(connectionString, e => e.MigrationsAssembly("Migrators.MsSql")),
            _ => throw new Exception($"DB Provider {dbProvider} is not supported.")
        };
}
