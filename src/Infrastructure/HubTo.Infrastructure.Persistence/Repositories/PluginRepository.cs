using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HubTo.Infrastructure.Persistence.Repositories;

internal sealed class PluginRepository : Repository<PluginEntity>, IPluginRepository
{
    public PluginRepository(HubToContext context) : base(context) { }

    public async Task<IReadOnlyList<PluginEntity>> GetActivePluginsWithSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await Query
            .AsNoTracking()
            .Include(x => x.PluginSettings)
            .ToListAsync(cancellationToken);
    }

    public async Task<PluginEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Query
            .AsNoTracking()
            .Include(x => x.PluginSettings)
            .Where(x => x.Name == name)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
