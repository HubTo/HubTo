using HubTo.Abstraction.Registrars;
using System.Reflection;
using System.Runtime.Loader;

namespace HubTo.Core.Application.Contracts.Plugins;

public interface IPluginRegistry
{
    void Register(IHubToPlugin plugin, AssemblyLoadContext context, Func<AssemblyLoadContext, AssemblyName, Assembly?>? resolver = null);
    bool TryUnregister(string pluginName);
    IEnumerable<IHubToPlugin> AllPlugins { get; }
    IRegistrarPlugin? GetPlugin(string path, string method);
    IStoragePlugin? GetDefaultStorage();
    bool IsReloading { get; }
    void SetReloading(bool value);
}

