using HubTo.Core.Application.Common.CQRS;

namespace HubTo.Core.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand
(
    string Username,
    string Password
) : ICommand<LoginDto>;
