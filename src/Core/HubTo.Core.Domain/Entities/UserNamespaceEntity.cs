using HubTo.Core.Domain.SeedWork;
using HubTo.Core.Domain.SeedWork.Enums;

namespace HubTo.Core.Domain.Entities;

public class UserNamespaceEntity : BaseEntity
{
    public NamespaceRole NamespaceRole { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public Guid NamespaceId { get; set; }
    public UserEntity User { get; set; } = default!;
    public NamespaceEntity Namespace { get; set; } = default!;
}
