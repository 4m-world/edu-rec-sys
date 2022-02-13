global using Ardalis.Specification;
global using CodeMatrix.Mepd.Application.Common.Events;
global using CodeMatrix.Mepd.Application.Common.Exceptions;
global using CodeMatrix.Mepd.Application.Common.FileStorage;
global using CodeMatrix.Mepd.Application.Common.Interfaces;
global using CodeMatrix.Mepd.Application.Common.Models;
global using CodeMatrix.Mepd.Application.Common.Persistence;
global using CodeMatrix.Mepd.Application.Common.Specifications;
global using CodeMatrix.Mepd.Application.Common.Validation;
global using CodeMatrix.Mepd.Domain.Common.Contracts;
global using CodeMatrix.Mepd.Shared.Notifications;
global using FluentValidation;
global using MediatR;
global using Microsoft.Extensions.Localization;
global using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CodeMatrix.Mepd.Application
{
    public static class Startup
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return services
                .AddValidatorsFromAssembly(assembly)
                .AddMediatR(assembly);
        }
    }
}
