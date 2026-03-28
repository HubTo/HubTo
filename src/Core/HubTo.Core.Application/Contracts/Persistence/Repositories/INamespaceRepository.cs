using HubTo.Core.Domain.Entities;

namespace HubTo.Core.Application.Contracts.Persistence.Repositories;

public interface INamespaceRepository : IRepository<NamespaceEntity>
{
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<NamespaceEntity?> GetActiveByIdAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<NamespaceEntity?> GetActiveBySlugAsync(string slug, CancellationToken cancellationToken = default);
}