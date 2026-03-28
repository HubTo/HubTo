using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubTo.Infrastructure.Persistence.Configurations;

internal sealed class ArtifactConfiguration : BaseConfiguration<ArtifactEntity>
{
    public override void Configure(EntityTypeBuilder<ArtifactEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("ARTIFACTS");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("NAME");

        builder.Property(x => x.RegistrarType)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("REGISTRAR_TYPE");

        builder.Property(x => x.NamespaceId)
            .IsRequired()
            .HasColumnName("NAMESPACE_ID");

        builder.HasOne(x => x.Namespace)
            .WithMany(x => x.Artifacts)
            .HasForeignKey(x => x.NamespaceId);

        builder.HasIndex(x => new { x.NamespaceId, x.Name, x.RegistrarType })
            .IsUnique();
    }
}