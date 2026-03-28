using HubTo.Core.Domain.SeedWork;
using HubTo.Core.Domain.SeedWork.Enums;

namespace HubTo.Core.Domain.Entities;

public class ApiKeyEntity : BaseEntity
{
    public string Label { get; set; } = default!;
    public string KeyHash { get; set; } = default!;
    public string Prefix { get; set; } = default!;
    public ApiKeyPermission Permission { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public bool IsRevoked { get; set; }
    public Guid UserId { get; set; }
    public Guid NamespaceId { get; set; }
    public UserEntity User { get; set; } = default!;
    public NamespaceEntity Namespace { get; set; } = default!;
}
