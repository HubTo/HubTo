using HubTo.Core.Domain.SeedWork;

namespace HubTo.Core.Domain.Entities;

public sealed class PluginEntity : BaseEntity
{
    public string Name { get; set; } = null!;
    public string AssemblyName { get; set; } = default!;
    public int PluginTypeValue { get; set; }
    public bool IsEnabled { get; set; }

    public ICollection<PluginSettingEntity> PluginSettings { get; set; } = new List<PluginSettingEntity>();
}
