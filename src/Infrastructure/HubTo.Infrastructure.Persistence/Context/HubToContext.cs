using HubTo.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HubTo.Infrastructure.Persistence.Context;

public sealed class HubToContext : DbContext
{
    public HubToContext(DbContextOptions<HubToContext> context) : base(context)
    { }

    public DbSet<ApiKeyEntity> ApiKeys { get; set; }
    public DbSet<ArtifactEntity> Artifacts { get; set; }
    public DbSet<ArtifactVersionEntity> ArtifactVersions { get; set; }
    public DbSet<NamespaceEntity> Namespaces { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserNamespaceEntity> UserNamespaces { get; set; }
    public DbSet<PluginEntity> Plugins { get; set; }
    public DbSet<PluginSettingEntity> PluginSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HubToContext).Assembly);
    }
}
