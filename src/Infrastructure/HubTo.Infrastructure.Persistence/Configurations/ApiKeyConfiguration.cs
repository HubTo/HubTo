using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubTo.Infrastructure.Persistence.Configurations;

internal sealed class ApiKeyConfiguration : BaseConfiguration<ApiKeyEntity>
{
    public override void Configure(EntityTypeBuilder<ApiKeyEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("API_KEYS");

        builder.Property(x => x.Label)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("LABEL");

        builder.Property(x => x.KeyHash)
            .IsRequired()
            .HasMaxLength(512)
            .HasColumnName("KEY_HASH");

        builder.Property(x => x.Prefix)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("PREFIX");

        builder.Property(x => x.Permission)
            .IsRequired()
            .HasColumnName("PERMISSION");

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("EXPIRES_AT");

        builder.Property(x => x.LastUsedAt)
            .HasColumnName("LAST_USED_AT");

        builder.Property(x => x.IsRevoked)
            .IsRequired()
            .HasColumnName("IS_REVOKED");

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("USER_ID");

        builder.Property(x => x.NamespaceId)
            .IsRequired()
            .HasColumnName("NAMESPACE_ID");

        builder.HasOne(x => x.User)
            .WithMany(x => x.ApiKeys)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Namespace)
            .WithMany(x => x.ApiKeys)
            .HasForeignKey(x => x.NamespaceId);

        builder.HasIndex(x => x.Prefix);
        builder.HasIndex(x => x.KeyHash);
    }
}
