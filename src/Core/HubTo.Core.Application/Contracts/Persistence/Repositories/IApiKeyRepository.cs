using HubTo.Core.Domain.Entities;

namespace HubTo.Core.Application.Contracts.Persistence.Repositories;

public interface IApiKeyRepository : IRepository<ApiKeyEntity>
{
    Task<ApiKeyEntity?> GetByKeyAsync(string apiKey, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ApiKeyEntity>> GetByPrefixAsync(string prefix, CancellationToken ct = default);
}