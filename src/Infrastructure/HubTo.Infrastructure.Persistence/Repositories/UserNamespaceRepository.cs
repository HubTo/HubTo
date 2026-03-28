using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.Entities;
using HubTo.Core.Domain.SeedWork.Enums;
using HubTo.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HubTo.Infrastructure.Persistence.Repositories;

internal sealed class UserNamespaceRepository : Repository<UserNamespaceEntity>, IUserNamespaceRepository
{
    public UserNamespaceRepository(HubToContext context) : base(context)
    { }

    public async Task<int> CountAdminsAsync(Guid namespaceId, CancellationToken cancellationToken = default)
    {
        return await Query
            .AsNoTracking()
            .CountAsync(x => x.NamespaceId == namespaceId && x.NamespaceRole == NamespaceRole.Admin, cancellationToken);
    }

    public async Task<int> CountAdminsAsyncForUser(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Query
            .AsNoTracking()
            .CountAsync(x => x.UserId == userId && x.NamespaceRole == NamespaceRole.Admin, cancellationToken);
    }

    public async Task<UserNamespaceEntity?> GetByUserAndNamespaceAsync(Guid userId, Guid namespaceId, CancellationToken cancellationToken = default)
    {
        return await Query
            .AsNoTracking()
            .Include(x => x.Namespace)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.NamespaceId == namespaceId, cancellationToken);
    }

    public async Task<IReadOnlyList<UserNamespaceEntity>> GetMembersByNamespaceIdAsync(Guid namespaceId, CancellationToken cancellationToken = default)
    {
        return await Query
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.NamespaceId == namespaceId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserNamespaceEntity>> GetNamespacesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Query
            .AsNoTracking()
            .Include(x => x.Namespace)
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserNamespaceEntity?> GetTrackedByUserAndNamespaceAsync(Guid userId, Guid namespaceId, CancellationToken cancellationToken = default)
    {
        return await Query
            .Include(x => x.Namespace)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.NamespaceId == namespaceId, cancellationToken);
    }
}
