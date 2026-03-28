using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HubTo.Infrastructure.Persistence.Repositories;

internal sealed class NamespaceRepository : Repository<NamespaceEntity>, INamespaceRepository
{
    public NamespaceRepository(HubToContext context) : base(context)
    { }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Query.AsNoTracking().AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await Query.AsNoTracking().AnyAsync(x => x.Slug == slug, cancellationToken);
    }

    public async Task<NamespaceEntity?> GetActiveByIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await Query.FirstOrDefaultAsync(x => x.Id == organizationId, cancellationToken);
    }

    public async Task<NamespaceEntity?> GetActiveBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await Query.FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);
    }
}
