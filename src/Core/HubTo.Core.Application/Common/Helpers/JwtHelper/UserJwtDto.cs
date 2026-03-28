namespace HubTo.Core.Application.Common.Helpers.JwtHelper;

internal sealed record UserJwtDto
(
    Guid UserId,
    string Email,
    string Username,
    List<(Guid NamespaceId, int RoleId)>? Roles = null
)
{
    public List<(Guid NamespaceId, int RoleId)> Roles { get; init; } = Roles ?? new();
}