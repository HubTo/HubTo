using HubTo.Core.Application.Common.Behaviors;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HubTo.Core.Application.Extensions;

internal static class MediatorExtensions
{
    internal static IServiceCollection AddMediatrRegistry(this IServiceCollection services)
    {
        var assm = Assembly.GetExecutingAssembly();

        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return services;
    }
}