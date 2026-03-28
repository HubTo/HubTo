using HubTo.Abstraction.Registrars;
using HubTo.Core.Application.Contracts.Plugins;

namespace HubTo.Infrastructure.Plugins.Services;

public sealed class PluginRegistry : IPluginRegistry
{
    private readonly List<IHubToPlugin> _plugins = new();

    public void Register(IHubToPlugin plugin)
    {
        if (plugin != null)
        {
            _plugins.Add(plugin);
        }
    }

    public IEnumerable<IHubToPlugin> AllPlugins => _plugins;

    public IHubToPlugin? GetPlugin(string path, string method)
        => _plugins.FirstOrDefault(p => p.CanHandle(path, method));

    public IStoragePlugin? GetDefaultStorage()
    => _plugins.OfType<IStoragePlugin>().FirstOrDefault();
}