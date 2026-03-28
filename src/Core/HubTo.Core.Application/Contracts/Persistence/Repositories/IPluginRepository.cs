using HubTo.Core.Domain.Entities;

namespace HubTo.Core.Application.Contracts.Persistence.Repositories;

public interface IPluginRepository : IRepository<PluginEntity>
{
    Task<IReadOnlyList<PluginEntity>> GetActivePluginsWithSettingsAsync(CancellationToken cancellationToken = default);
}
