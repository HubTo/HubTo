using HubTo.Abstraction.Registrars;
using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Application.Contracts.Plugins;
using HubTo.Core.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.Loader;

namespace HubTo.Infrastructure.Plugins.Services;

public sealed class PluginLoader : IPluginLoader
{
    private readonly IPluginRepository _pluginRepository;
    private readonly IPluginRegistry _registry;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<PluginLoader> _logger;
    private readonly string _pluginBaseDirectory;

    public PluginLoader(
        IPluginRepository pluginRepo,
        IPluginRegistry registry,
        ILoggerFactory loggerFactory,
        ILogger<PluginLoader> logger)
    {
        _pluginRepository = pluginRepo;
        _registry = registry;
        _loggerFactory = loggerFactory;
        _logger = logger;
        _pluginBaseDirectory = Path.Combine(AppContext.BaseDirectory, "plugins");
    }

    public async Task InitializePluginsAsync(CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_pluginBaseDirectory))
            Directory.CreateDirectory(_pluginBaseDirectory);

        var pluginsFromDb = await _pluginRepository.GetActivePluginsWithSettingsAsync(cancellationToken);

        foreach (var dbPlugin in pluginsFromDb)
            await LoadAsync(dbPlugin, cancellationToken);
    }

    public async Task<bool> ReloadPluginAsync(CancellationToken cancellationToken = default)
    {
        _registry.SetReloading(true);
        try
        {
            foreach (var plugin in _registry.AllPlugins.ToList())
            {
                await plugin.ShutdownAsync(cancellationToken);
                _registry.TryUnregister(plugin.Name);
            }

            var pluginsFromDb = await _pluginRepository.GetActivePluginsWithSettingsAsync(cancellationToken);
            var results = new List<bool>();

            foreach (var dbPlugin in pluginsFromDb)
                results.Add(await LoadAsync(dbPlugin, cancellationToken));

            return results.All(r => r);
        }
        finally
        {
            _registry.SetReloading(false);
        }
    }

    private async Task<bool> LoadAsync(PluginEntity dbPlugin, CancellationToken cancellationToken)
    {
        var pluginFolder = Path.Combine(_pluginBaseDirectory, dbPlugin.Name);
        var dllPath = Path.Combine(pluginFolder, dbPlugin.AssemblyName);

        if (!File.Exists(dllPath))
        {
            _logger.LogWarning("Plugin DLL not found, skipping. Plugin: {PluginName}, Path: {DllPath}", dbPlugin.Name, dllPath);
            return false;
        }

        try
        {
            var loadContext = new AssemblyLoadContext(dbPlugin.Name, isCollectible: true);

            Func<AssemblyLoadContext, AssemblyName, Assembly?> resolver = (context, assemblyName) =>
            {
                if (assemblyName.Name == "HubTo.Abstraction") return null;
                var expectedPath = Path.Combine(pluginFolder, $"{assemblyName.Name}.dll");
                if (!File.Exists(expectedPath)) return null;

                using var depStream = new MemoryStream(File.ReadAllBytes(expectedPath));
                return context.LoadFromStream(depStream);
            };

            loadContext.Resolving += resolver;

            using var stream = new MemoryStream(File.ReadAllBytes(dllPath));
            var assembly = loadContext.LoadFromStream(stream);

            var pluginType = assembly.GetTypes()
                .FirstOrDefault(t => typeof(IHubToPlugin).IsAssignableFrom(t)
                                     && !t.IsInterface && !t.IsAbstract);

            if (pluginType is null)
            {
                loadContext.Resolving -= resolver;
                loadContext.Unload();
                _logger.LogWarning("No valid IHubToPlugin found. Plugin: {PluginName}", dbPlugin.Name);
                return false;
            }

            if (Activator.CreateInstance(pluginType) is not IHubToPlugin instance)
            {
                loadContext.Resolving -= resolver;
                loadContext.Unload();
                _logger.LogWarning("Failed to create instance. Plugin: {PluginName}", dbPlugin.Name);
                return false;
            }

            var settings = dbPlugin.PluginSettings.ToDictionary(s => s.Key, s => s.Value);
            var coreLogger = _loggerFactory.CreateLogger($"Plugin:{dbPlugin.Name}");
            var adapter = new HubToLoggerAdapter(coreLogger);

            await instance.InitializeAsync(settings, adapter, cancellationToken);
            _registry.Register(instance, loadContext, resolver);

            _logger.LogInformation("Plugin loaded successfully. Plugin: {PluginName}", dbPlugin.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load plugin. Plugin: {PluginName}", dbPlugin.Name);
            return false;
        }
    }
}
