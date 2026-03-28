using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Context;

namespace HubTo.Infrastructure.Persistence.Repositories;

internal sealed class PluginSettingRepository : Repository<PluginSettingEntity>, IPluginSettingRepository
{
    public PluginSettingRepository(HubToContext context) : base(context) { }
}
