using HubTo.Core.Domain.Entities;

namespace HubTo.Core.Application.Contracts.Persistence.Repositories;

public interface IArtifactVersionRepository : IRepository<ArtifactVersionEntity>
{
    Task<IReadOnlyList<ArtifactVersionEntity>> GetPackageVersionsByArtifactIdAsync(Guid ArtifactId, CancellationToken cancellationToken = default);
}
