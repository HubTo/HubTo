using HubTo.Abstraction.Registrars;
using HubTo.Core.Application.Contracts.Plugins;
using System.Reflection;
using System.Runtime.Loader;

namespace HubTo.Infrastructure.Plugins.Services;

public sealed class PluginRegistry : IPluginRegistry
{
    private readonly List<PluginEntry> _plugins = new();
    public bool IsReloading { get; private set; }

    public void SetReloading(bool value) => IsReloading = value;

    public void Register(IHubToPlugin plugin, AssemblyLoadContext context, Func<AssemblyLoadContext, AssemblyName, Assembly?>? resolver = null)
    {
        if (plugin != null)
            _plugins.Add(new PluginEntry(plugin, context, resolver));
    }

    public bool TryUnregister(string pluginName)
    {
        var entry = _plugins.FirstOrDefault(p => p.Plugin.Name == pluginName);
        if (entry is null) return false;

        _plugins.Remove(entry);

        if (entry.Resolver != null)
            entry.Context.Resolving -= entry.Resolver;

        var weakRef = new WeakReference(entry.Context);
        entry.Context.Unload();

        for (int i = 0; i < 10 && weakRef.IsAlive; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        return true;
    }

    public IEnumerable<IHubToPlugin> AllPlugins => _plugins.Select(p => p.Plugin);

    public IRegistrarPlugin? GetPlugin(string path, string method)
        => _plugins
            .Select(p => p.Plugin)
            .OfType<IRegistrarPlugin>()
            .FirstOrDefault(p => p.CanHandle(path, method));

    public IStoragePlugin? GetDefaultStorage()
        => _plugins.Select(p => p.Plugin).OfType<IStoragePlugin>().FirstOrDefault();

    private sealed class PluginEntry
    {
        public IHubToPlugin Plugin { get; }
        public AssemblyLoadContext Context { get; }
        public Func<AssemblyLoadContext, AssemblyName, Assembly?>? Resolver { get; }

        public PluginEntry(IHubToPlugin plugin, AssemblyLoadContext context, Func<AssemblyLoadContext, AssemblyName, Assembly?>? resolver)
        {
            Plugin = plugin;
            Context = context;
            Resolver = resolver;
        }
    }
}
