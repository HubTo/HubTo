namespace HubTo.Core.Application.Features.Auth.Commands.Login;

public sealed record LoginDto
(
    string AccessToken,
    DateTime ExpiresAt
);
