namespace HubTo.Core.Application.Contracts.Plugins;

public interface IPluginLoader
{
    Task InitializePluginsAsync(CancellationToken ct = default);
}
