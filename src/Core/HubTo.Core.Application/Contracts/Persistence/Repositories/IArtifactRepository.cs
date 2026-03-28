using HubTo.Core.Domain.Entities;

namespace HubTo.Core.Application.Contracts.Persistence.Repositories;

public interface IArtifactRepository : IRepository<ArtifactEntity>
{
    Task<ArtifactEntity?> GetByPackageAsync(Guid namespaceId, string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid namespaceId, string name, CancellationToken cancellationToken = default);
}