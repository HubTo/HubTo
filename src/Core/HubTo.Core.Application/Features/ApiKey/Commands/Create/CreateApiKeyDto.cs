namespace HubTo.Core.Application.Features.ApiKey.Commands.Create;

public sealed record CreateApiKeyDto
(
    Guid Id,
    string Label,
    string PlainTextKey
);
