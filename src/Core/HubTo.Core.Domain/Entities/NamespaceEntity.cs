using HubTo.Core.Domain.SeedWork;

namespace HubTo.Core.Domain.Entities;

public class NamespaceEntity : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }
    public ICollection<UserNamespaceEntity> UserNamespaces { get; set; } = new List<UserNamespaceEntity>();
    public ICollection<ArtifactEntity> Artifacts { get; set; } = new List<ArtifactEntity>();
    public ICollection<ApiKeyEntity> ApiKeys { get; set; } = new List<ApiKeyEntity>();
}
