using HubTo.Core.Domain.SeedWork;

namespace HubTo.Core.Domain.Entities;

public sealed class PluginSettingEntity : BaseEntity
{
    public Guid PluginId { get; set; }
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;

    public PluginEntity Plugin { get; set; } = null!;
}
