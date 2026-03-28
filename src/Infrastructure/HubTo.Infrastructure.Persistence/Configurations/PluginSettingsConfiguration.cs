using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubTo.Infrastructure.Persistence.Configurations;

internal sealed class PluginSettingsConfiguration : BaseConfiguration<PluginSettingEntity>
{
    public override void Configure(EntityTypeBuilder<PluginSettingEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("PLUGIN_SETTINGS");

        builder.Property(x => x.Key)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("KEY");

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(512)
            .HasColumnName("VALUE");

        builder.Property(x => x.PluginId)
            .IsRequired()
            .HasColumnName("PLUGIN_ID");

        builder.HasOne(x => x.Plugin)
            .WithMany(x => x.PluginSettings)
            .HasForeignKey(x => x.PluginId)
            .HasConstraintName("FK_PLUGIN_SETTINGS_PLUGIN");
    }
}
