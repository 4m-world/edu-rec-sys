global using CodeMatrix.Mepd.Application.Common.Interfaces;
global using CodeMatrix.Mepd.Application.Common.Models;
global using CodeMatrix.Mepd.Infrastructure.Auth.Permissions;
global using CodeMatrix.Mepd.Infrastructure.OpenApi;
global using CodeMatrix.Mepd.Shared.Authorization;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using NSwag.Annotations;
using CodeMatrix.Mepd.Application;
using CodeMatrix.Mepd.Host.Configurations;
using CodeMatrix.Mepd.Infrastructure;
using FluentValidation.AspNetCore;
using Serilog;

namespace CodeMatrix.Mepd.Host
{
    /// <summary>
    /// Prgramme entry point
    /// </summary>
    public class Programme
    {
        /// <summary>
        /// Main application entry point
        /// </summary>
        /// <param name="args">Application arguments</param>
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Log.Information("Server Booting Up...");
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Keep the previous behavior of allowing an unhandled exception in a BackgroundService to not stop the Host
                builder.Host.ConfigureServices(services =>
                {
                    services.Configure<HostOptions>(options =>
                    {
                        options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
                    });
                });

                builder.Host.AddConfigurations();
                builder.Host.UseSerilog((_, config) =>
                {
                    config.WriteTo.Console()
                    .ReadFrom.Configuration(builder.Configuration);
                });

                builder.Services.AddApplication();
                builder.Services.AddInfrastructure(builder.Configuration);
                builder.Services.AddControllers().AddFluentValidation();

                var app = builder.Build();

                await app.Services.InitializeDatabasesAsync();

                app.UseInfrastructure(builder.Configuration);
                app.MapEndpoints();

                app.Run();
            }
            catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
            {
                Log.Fatal(ex, "Unhandled exception");
            }
            finally
            {
                Log.Information("Server Shutting down...");
                Log.CloseAndFlush();
            }
        }
    }
}