using HubTo.Core.Application.Common.CQRS;

namespace HubTo.Core.Application.Features.ApiKey.Commands.Create;

public sealed record CreateApiKeyCommand
(
    string Label,
    Guid NamespaceId,
    DateTime? ExpiresAt,
    int Permission
) : ICommand<CreateApiKeyDto>;