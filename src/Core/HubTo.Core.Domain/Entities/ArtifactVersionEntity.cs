using HubTo.Core.Domain.SeedWork;

namespace HubTo.Core.Domain.Entities;

public class ArtifactVersionEntity : BaseEntity
{
    public string Version { get; set; } = default!;
    public string StoragePath { get; set; } = default!;
    public string? Digest { get; set; }
    public string? MediaType { get; set; }
    public long? SizeInBytes { get; set; }
    public bool IsListed { get; set; } = true;
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    public Guid ArtifactId { get; set; }
    public ArtifactEntity Artifact { get; set; } = default!;
}
