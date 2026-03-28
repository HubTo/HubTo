using HubTo.Core.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HubTo.Infrastructure.Persistence.Configurations.Base;

internal abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(x => x.Id)
           .ValueGeneratedNever()
           .HasColumnName("ID");

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasColumnName("CREATED_BY");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("CREATED_AT");

        builder.Property(x => x.UpdatedBy)
            .IsRequired()
            .HasColumnName("UPDATED_BY");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasColumnName("UPDATED_AT");

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("IS_DELETED");

        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasIndex(x => x.IsDeleted);
    }
}
