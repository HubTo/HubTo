using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubTo.Infrastructure.Persistence.Configurations;

internal sealed class PluginConfiguration : BaseConfiguration<PluginEntity>
{
    public override void Configure(EntityTypeBuilder<PluginEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("PLUGINS");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("NAME");

        builder.Property(x => x.AssemblyName)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("ASSEMBLY_NAME");

        builder.Property(x => x.PluginTypeValue)
            .HasColumnName("PLUGIN_TYPE_VALUE");

        builder.Property(x => x.IsEnabled)
            .HasColumnName("IS_ENABLED");
    }
}
