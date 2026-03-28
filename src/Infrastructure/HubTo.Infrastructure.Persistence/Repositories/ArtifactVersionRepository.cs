using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Context;

namespace HubTo.Infrastructure.Persistence.Repositories;

internal sealed class ArtifactVersionRepository : Repository<ArtifactVersionEntity>, IArtifactVersionRepository
{
    public ArtifactVersionRepository(HubToContext context) : base(context)
    { }

    public async Task<IReadOnlyList<ArtifactVersionEntity>> GetPackageVersionsByArtifactIdAsync(Guid ArtifactId, CancellationToken cancellationToken = default)
    {
        return (await WhereAsync(x => x.ArtifactId == ArtifactId, cancellationToken)).ToList();
    }
}
