using CodeMatrix.Mepd.Application.Common.Interfaces;
using CodeMatrix.Mepd.Application.Common.Interfaces.Csv;
using CodeMatrix.Mepd.Infrastructure.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using PuppeteerSharp;
using RazorLight;

namespace CodeMatrix.Mepd.Infrastructure.Common;

internal static class Startup
{
    internal static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddServices(typeof(ITransientService), ServiceLifetime.Transient)
            .AddServices(typeof(IScopedService), ServiceLifetime.Scoped);        

        var engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(Environment.CurrentDirectory)
            .UseMemoryCachingProvider()
            .Build();
        services
            .AddSingleton<IRazorLightEngine>(engine)
            .AddSingleton<IHtmlGenerator, HtmlGenerator>();

        var browserFetcher = new BrowserFetcher();
        browserFetcher.DownloadAsync().GetAwaiter().GetResult();

        services.AddSingleton<IPdfConverter, PuppeteerSharpConverter>();

        services.AddScoped(typeof(ICsvReader<>), typeof(CsvReader<>));
        services.AddScoped(typeof(ICsvWriter<>), typeof(CsvWriter<>));

        return services;
    }

    internal static IServiceCollection AddServices(this IServiceCollection services, Type interfaceType, ServiceLifetime lifetime)
    {
        var interfaceTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .Select(t => new
            {
                Service = t.GetInterfaces().FirstOrDefault(),
                Implementation = t
            })
            .Where(t => t.Service is not null && interfaceType.IsAssignableFrom(t.Service));

        foreach (var type in interfaceTypes)
        {
            services.AddService(type.Service!, type.Implementation, lifetime);
        }

        return services;
    }

    internal static IServiceCollection AddService(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        return lifetime switch
        {
            ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
            ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
            ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
            _ => throw new ArgumentException("Invalid lifeTime", nameof(lifetime))
        };
    }
}