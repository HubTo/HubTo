using HubTo.Core.Domain.Entities;

namespace HubTo.Core.Application.Contracts.Persistence.Repositories;

public interface IUserNamespaceRepository : IRepository<UserNamespaceEntity>
{
    Task<UserNamespaceEntity?> GetByUserAndNamespaceAsync(Guid userId, Guid namespaceId, CancellationToken cancellationToken = default);
    Task<UserNamespaceEntity?> GetTrackedByUserAndNamespaceAsync(Guid userId, Guid namespaceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserNamespaceEntity>> GetNamespacesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserNamespaceEntity>> GetMembersByNamespaceIdAsync(Guid namespaceId, CancellationToken cancellationToken = default);
    Task<int> CountAdminsAsync(Guid namespaceId, CancellationToken cancellationToken = default);
    Task<int> CountAdminsAsyncForUser(Guid userId, CancellationToken cancellationToken = default);
}
