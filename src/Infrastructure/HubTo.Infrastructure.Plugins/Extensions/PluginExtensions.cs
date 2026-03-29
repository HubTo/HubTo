using HubTo.Abstraction.Auth;
using HubTo.Core.Application.Contracts.Plugins;
using HubTo.Infrastructure.Plugins.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HubTo.Infrastructure.Plugins.Extensions;

public static class PluginExtensions
{
    public static IServiceCollection AddPluginsServices(this IServiceCollection services)
    {
        services.AddSingleton<IPluginRegistry, PluginRegistry>();
        services.AddScoped<IHubToAuthValidator, HubToAuthValidator>();

        services.AddScoped<IPluginLoader, PluginLoader>();

        return services;
    }

    public static async Task<WebApplication> AddPlugins(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var loader = scope.ServiceProvider.GetRequiredService<IPluginLoader>();
        await loader.InitializePluginsAsync();

        return app;
    }
}