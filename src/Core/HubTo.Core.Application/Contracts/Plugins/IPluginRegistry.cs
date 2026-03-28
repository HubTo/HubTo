using HubTo.Abstraction.Registrars;

namespace HubTo.Core.Application.Contracts.Plugins;

public interface IPluginRegistry
{
    void Register(IHubToPlugin plugin);
    IHubToPlugin? GetPlugin(string path, string method);
    IEnumerable<IHubToPlugin> AllPlugins { get; }
    IStoragePlugin? GetDefaultStorage();
}