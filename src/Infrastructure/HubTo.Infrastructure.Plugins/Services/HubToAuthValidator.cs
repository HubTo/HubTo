using HubTo.Abstraction.Auth;
using HubTo.Core.Application.Common.Helpers.Argon2Helper;
using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.SeedWork.Enums;

namespace HubTo.Infrastructure.Plugins.Services;

internal class HubToAuthValidator : IHubToAuthValidator
{
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly IArgon2Helper _argon2Helper;
    private readonly IUserRepository _userRepository;

    public HubToAuthValidator(
        IApiKeyRepository apiKeyRepository,
        IArgon2Helper argon2Helper,
        IUserRepository userRepository)
    {
        _apiKeyRepository = apiKeyRepository;
        _argon2Helper = argon2Helper;
        _userRepository = userRepository;
    }

    public async Task<HubToAuthResult> ValidateAsync(HubToAuthRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Scheme))
            return HubToAuthResult.Failure("Invalid auth request");

        switch (request.Scheme)
        {
            case "ApiKey":
                return await ValidateApiKeyAsync(request, cancellationToken);

            case "Basic":
                return await ValidateBasicAsync(request, cancellationToken);

            default:
                return HubToAuthResult.Failure("Unsupported authentication scheme");
        }
    }

    private async Task<HubToAuthResult> ValidateApiKeyAsync(HubToAuthRequest request, CancellationToken cancellationToken)
    {
        if (!request.Values.TryGetValue("apiKey", out var apiKey) ||
            string.IsNullOrWhiteSpace(apiKey) || apiKey.Length < 12)
        {
            return HubToAuthResult.Failure("Invalid API key format");
        }

        var prefix = apiKey.Substring(0, 12);
        var candidates = await _apiKeyRepository.GetByPrefixAsync(prefix, cancellationToken);

        var key = candidates.FirstOrDefault(x => _argon2Helper.Verify(apiKey, x.KeyHash));

        if (key is null)
        {
            _argon2Helper.Verify("cO8Gszoo69NnpamDWbZCm047vHBR2U9s", "$argon2id$v=19$m=32768,t=3,p=2$6Owth8NxVLmZXdsfKmhrIQ==$h7x1EjXRKSXdluIcBB3teH5vGXY2MFHuPbFoXjxRPt4=");
            return HubToAuthResult.Failure("Invalid API key");
        }

        var scope = key.Permission == ApiKeyPermission.Write ? "Write" : "Read";

        return HubToAuthResult.Success(
            key.UserId.ToString(),
            key.NamespaceId.ToString(),
            scope);
    }

    private async Task<HubToAuthResult> ValidateBasicAsync(HubToAuthRequest request, CancellationToken cancellationToken)
    {
        if (!request.Values.TryGetValue("username", out var username) ||
            !request.Values.TryGetValue("password", out var password) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) ||
            password.Length < 12)
        {
            return HubToAuthResult.Failure("Invalid credentials");
        }

        var normalizedUsername = username.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByUsernameAsync(normalizedUsername, cancellationToken);

        if (user is null)
        {
            // timing attack mitigation
            _argon2Helper.Verify(password, "$argon2id$v=19$m=32768,t=3,p=2$6Owth8NxVLmZXdsfKmhrIQ==$h7x1EjXRKSXdluIcBB3teH5vGXY2MFHuPbFoXjxRPt4=");
            return HubToAuthResult.Failure("Username or password is incorrect.");
        }

        var prefix = password.Substring(0, 12);
        var candidates = await _apiKeyRepository.GetByPrefixAsync(prefix, cancellationToken);

        var key = candidates.FirstOrDefault(x => _argon2Helper.Verify(password, x.KeyHash));

        if (key is null || key.UserId != user.Id)
            return HubToAuthResult.Failure("Username or password is incorrect.");

        var scope = key.Permission == ApiKeyPermission.Write ? "Write" : "Read";

        return HubToAuthResult.Success(
            key.UserId.ToString(),
            key.NamespaceId.ToString(),
            scope);
    }
}