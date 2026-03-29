namespace HubTo.Core.Application.Contracts.Plugins;

public interface IPluginLoader
{
    Task InitializePluginsAsync(CancellationToken cancellationToken = default);
    Task<bool> ReloadPluginAsync(CancellationToken cancellationToken = default);
}
