using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HubTo.Infrastructure.Persistence.Repositories;

internal sealed class ArtifactRepository : Repository<ArtifactEntity>, IArtifactRepository
{
    public ArtifactRepository(HubToContext context) : base(context)
    { }

    public async Task<bool> ExistsAsync(Guid namespaceId, string name, CancellationToken cancellationToken = default)
    {
        return await Query
            .AsNoTracking()
            .AnyAsync(x => x.NamespaceId == namespaceId && x.Name == name, cancellationToken);
    }

    public async Task<ArtifactEntity?> GetByPackageAsync(Guid namespaceId, string name, CancellationToken cancellationToken = default)
    {
        return await Query
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NamespaceId == namespaceId && x.Name == name, cancellationToken);
    }
}
