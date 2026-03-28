using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubTo.Infrastructure.Persistence.Configurations;

internal sealed class NamespaceConfiguration : BaseConfiguration<NamespaceEntity>
{
    public override void Configure(EntityTypeBuilder<NamespaceEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("NAMESPACES");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("NAME");

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .HasColumnName("DESCRIPTION");

        builder.HasIndex(x => x.Name).IsUnique();
    }
}