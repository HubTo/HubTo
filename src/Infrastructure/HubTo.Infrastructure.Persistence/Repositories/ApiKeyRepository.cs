using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HubTo.Infrastructure.Persistence.Repositories;

internal sealed class ApiKeyRepository : Repository<ApiKeyEntity>, IApiKeyRepository
{
    public ApiKeyRepository(HubToContext context) : base(context)
    { }

    public async Task<ApiKeyEntity?> GetByKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        return await Query
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.KeyHash == apiKey && !x.IsRevoked && (x.ExpiresAt == null || x.ExpiresAt > DateTime.UtcNow), cancellationToken);
    }

    public async Task<IReadOnlyList<ApiKeyEntity>> GetByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        return await Query
            .AsNoTracking()
            .Where(x => x.Prefix == prefix &&
                        !x.IsRevoked &&
                        (x.ExpiresAt == null || x.ExpiresAt > DateTime.UtcNow))
            .ToListAsync(ct);
    }
}
