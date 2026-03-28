using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Application.Contracts.Plugins;
using HubTo.Abstraction.Registrars;
using System.Reflection;
using System.Runtime.Loader;

namespace HubTo.Infrastructure.Plugins.Services;

public sealed class PluginLoader : IPluginLoader
{
    private readonly IPluginRepository _pluginRepository;
    private readonly IPluginRegistry _registry;
    private readonly string _pluginBaseDirectory;

    public PluginLoader(
        IPluginRepository pluginRepo,
        IPluginRegistry registry)
    {
        _pluginRepository = pluginRepo;
        _registry = registry;
        _pluginBaseDirectory = Path.Combine(AppContext.BaseDirectory, "plugins");
    }

    public async Task InitializePluginsAsync(CancellationToken ct = default)
    {
        if (!Directory.Exists(_pluginBaseDirectory))
            Directory.CreateDirectory(_pluginBaseDirectory);

        var pluginsFromDb = await _pluginRepository.GetActivePluginsWithSettingsAsync(ct);

        foreach (var dbPlugin in pluginsFromDb)
        {
            var dllPath = Path.Combine(_pluginBaseDirectory, dbPlugin.AssemblyName);

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"[INFO] Plugin file is not recovered: {dbPlugin.AssemblyName}");
                continue;
            }

            try
            {
                var loadContext = new AssemblyLoadContext(dbPlugin.Name, isCollectible: true);

                loadContext.Resolving += (context, assemblyName) =>
                {
                    var expectedPath = Path.Combine(_pluginBaseDirectory, $"{assemblyName.Name}.dll");
                    if (File.Exists(expectedPath))
                    {
                        return context.LoadFromAssemblyPath(expectedPath);
                    }
                    return null;
                };

                var assembly = loadContext.LoadFromAssemblyPath(dllPath);

                var pluginType = assembly.GetTypes()
                    .FirstOrDefault(t => typeof(IHubToPlugin).IsAssignableFrom(t)
                                         && !t.IsInterface
                                         && !t.IsAbstract);

                if (pluginType != null && Activator.CreateInstance(pluginType) is IHubToPlugin instance)
                {
                    var settings = dbPlugin.PluginSettings.ToDictionary(s => s.Key, s => s.Value);

                    await instance.InitializeAsync(settings);

                    _registry.Register(instance);
                }
            }
            catch (Exception ex)
            {
                var message = ex is ReflectionTypeLoadException re ? string.Join(", ", re.LoaderExceptions.Select(e => e?.Message)) : ex.Message;
                Console.WriteLine($"[Error] {dbPlugin.Name} can not loaded: {message}");
            }
        }
    }
}