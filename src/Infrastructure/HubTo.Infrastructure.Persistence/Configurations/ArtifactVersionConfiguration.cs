using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubTo.Infrastructure.Persistence.Configurations;

internal sealed class ArtifactVersionConfiguration : BaseConfiguration<ArtifactVersionEntity>
{
    public override void Configure(EntityTypeBuilder<ArtifactVersionEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("ARTIFACT_VERSIONS");

        builder.Property(x => x.Version)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("VERSION");

        builder.Property(x => x.StoragePath)
            .IsRequired()
            .HasMaxLength(1000)
            .HasColumnName("STORAGE_PATH");

        builder.Property(x => x.Digest)
            .HasMaxLength(255)
            .HasColumnName("DIGEST");

        builder.Property(x => x.MediaType)
            .HasMaxLength(255)
            .HasColumnName("MEDIA_TYPE");

        builder.Property(x => x.SizeInBytes)
            .HasColumnName("SIZE_IN_BYTES");

        builder.Property(x => x.IsListed)
            .IsRequired()
            .HasColumnName("IS_LISTED");

        builder.Property(x => x.PublishedAt)
            .IsRequired()
            .HasColumnName("PUBLISHED_AT");

        builder.Property(x => x.ArtifactId)
            .IsRequired()
            .HasColumnName("ARTIFACT_ID");

        builder.HasOne(x => x.Artifact)
            .WithMany(x => x.Versions)
            .HasForeignKey(x => x.ArtifactId);

        builder.HasIndex(x => new { x.ArtifactId, x.Version })
            .IsUnique();

        builder.HasIndex(x => x.ArtifactId);
    }
}
