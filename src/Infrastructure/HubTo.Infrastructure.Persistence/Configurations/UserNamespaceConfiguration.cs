using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubTo.Infrastructure.Persistence.Configurations;

internal sealed class UserNamespaceConfiguration : BaseConfiguration<UserNamespaceEntity>
{
    public override void Configure(EntityTypeBuilder<UserNamespaceEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("USER_NAMESPACES");

        builder.Property(x => x.NamespaceRole)
            .IsRequired()
            .HasColumnName("NAMESPACE_ROLE");

        builder.Property(x => x.JoinedAt)
            .IsRequired()
            .HasColumnName("JOINED_AT");

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("USER_ID");

        builder.Property(x => x.NamespaceId)
            .IsRequired()
            .HasColumnName("NAMESPACE_ID");

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserNamespaces)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Namespace)
            .WithMany(x => x.UserNamespaces)
            .HasForeignKey(x => x.NamespaceId);

        builder.HasIndex(x => new { x.UserId, x.NamespaceId })
            .IsUnique();
    }
}