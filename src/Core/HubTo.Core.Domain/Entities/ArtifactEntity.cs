using HubTo.Core.Domain.SeedWork;

namespace HubTo.Core.Domain.Entities;

public class ArtifactEntity : BaseEntity
{
    public string Name { get; set; } = default!;
    public string RegistrarType { get; set; } = default!;
    public Guid NamespaceId { get; set; }
    public NamespaceEntity Namespace { get; set; } = default!;
    public ICollection<ArtifactVersionEntity> Versions { get; set; } = new List<ArtifactVersionEntity>();
}
