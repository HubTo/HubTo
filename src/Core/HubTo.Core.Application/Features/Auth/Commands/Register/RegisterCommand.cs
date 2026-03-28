using HubTo.Core.Application.Common.CQRS;

namespace HubTo.Core.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Username,
    string Email,
    string Password
) : ICommand<RegisterDto>;
