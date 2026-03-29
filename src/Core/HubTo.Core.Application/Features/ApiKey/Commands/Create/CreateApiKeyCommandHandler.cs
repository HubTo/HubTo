using HubTo.Core.Application.Common.CQRS;
using HubTo.Core.Application.Common.Helpers.Argon2Helper;
using HubTo.Core.Application.Common.Results;
using HubTo.Core.Application.Contracts;
using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.Entities;
using HubTo.Core.Domain.SeedWork.Enums;
using System.Security.Cryptography;

namespace HubTo.Core.Application.Features.ApiKey.Commands.Create;

internal sealed class CreateApiKeyCommandHandler : ICommandHandler<CreateApiKeyCommand, CreateApiKeyDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly INamespaceRepository _namespaceRepository;
    private readonly IClientContext _clientContext;
    private readonly IArgon2Helper _argon2Helper;

    public CreateApiKeyCommandHandler(
        IUserRepository userRepository,
        IApiKeyRepository apiKeyRepository,
        INamespaceRepository namespaceRepository,
        IClientContext clientContext,
        IArgon2Helper argon2Helper)
    {
        _userRepository = userRepository;
        _apiKeyRepository = apiKeyRepository;
        _namespaceRepository = namespaceRepository;
        _clientContext = clientContext;
        _argon2Helper = argon2Helper;
    }

    public async ValueTask<Result<CreateApiKeyDto>> Handle(CreateApiKeyCommand request, CancellationToken cancellationToken)
    {
        if (!_clientContext.UserId.HasValue || _clientContext.UserId == Guid.Empty)
            return Result<CreateApiKeyDto>.Fail(new List<string> { "Token is not valid." });

        var userEntity = await _userRepository.GetByIdAsync(_clientContext.UserId.Value, cancellationToken);
        if (userEntity is null)
            return Result<CreateApiKeyDto>.Fail(new List<string> { "User is not found." });
        if (!userEntity.IsActive)
            return Result<CreateApiKeyDto>.Fail(new List<string> { "Account is inactive." });

        var namespaceEntity = await _namespaceRepository.GetByIdAsync(request.NamespaceId, cancellationToken);
        if (namespaceEntity is null)
            return Result<CreateApiKeyDto>.Fail(new List<string> { "Namespace is not found." });

        if (!Enum.GetValues<ApiKeyPermission>().Contains((ApiKeyPermission)request.Permission))
            return Result<CreateApiKeyDto>.Fail(new List<string> { "Permission is not found." });

        var permission = (ApiKeyPermission)request.Permission;
        var plainTextKey = $"hubto_{Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).Replace('+', '-').Replace('/', '_').TrimEnd('=')}";

        var prefix = plainTextKey[..12];

        var apiKey = new ApiKeyEntity
        {
            UserId = userEntity.Id,
            NamespaceId = namespaceEntity.Id,
            Label = request.Label.Trim(),
            Prefix = prefix,
            KeyHash = _argon2Helper.Hash(plainTextKey),
            Permission = permission,
            ExpiresAt = request.ExpiresAt,
            IsRevoked = false,
        };

        await _apiKeyRepository.AddAsync(apiKey, cancellationToken);

        return Result<CreateApiKeyDto>.Ok(new CreateApiKeyDto(apiKey.Id, apiKey.Label, plainTextKey));
    }
}