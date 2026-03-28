using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubTo.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : BaseConfiguration<UserEntity>
{
    public override void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        base.Configure(builder);

        builder.ToTable("USERS");

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("USERNAME");

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(320)
            .HasColumnName("EMAIL");

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("PASSWORD_HASH");

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasColumnName("IS_ACTIVE");

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.Username).IsUnique();
    }
}