using HubTo.Core.Domain.SeedWork;

namespace HubTo.Core.Domain.Entities;

public class UserEntity : BaseEntity
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public bool IsActive { get; set; } = true;
    public ICollection<ApiKeyEntity> ApiKeys { get; set; } = new List<ApiKeyEntity>();
    public ICollection<UserNamespaceEntity> UserNamespaces { get; set; } = new List<UserNamespaceEntity>();
}
