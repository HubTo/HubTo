using HubTo.Core.Application.Common.CQRS;

namespace HubTo.Core.Application.Features.Namespace.Commands.Create;

public sealed record CreateNamespaceCommand
(
    string Name,
    string? Description
) : ICommand<CreateNamespaceDto>;

