using HubTo.Abstraction.Auth;
using HubTo.Core.Application.Common.Helpers.Argon2Helper;
using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.SeedWork.Enums;

namespace HubTo.Infrastructure.Plugins.Services;

internal class HubToAuthValidator : IHubToAuthValdiator
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

    public async Task<HubToAuthResult> ValidateApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey) || apiKey.Length < 12)
            return HubToAuthResult.Failure("Invalid API key format");

        var prefix = apiKey.Substring(0, 12);
        var candidates = await _apiKeyRepository.GetByPrefixAsync(prefix, cancellationToken);

        var key = candidates.FirstOrDefault(x => _argon2Helper.Verify(apiKey, x.KeyHash));

        if (key is null)
        {
            _argon2Helper.Verify("cO8Gszoo69NnpamDWbZCm047vHBR2U9s", "$argon2id$v=19$m=32768,t=3,p=2$6Owth8NxVLmZXdsfKmhrIQ==$h7x1EjXRKSXdluIcBB3teH5vGXY2MFHuPbFoXjxRPt4=");
            return HubToAuthResult.Failure("Invalid API key");
        }

        var topPermission = key.Permission == ApiKeyPermission.Write ? "Write" : "Read";

        return HubToAuthResult.Success(key.UserId.ToString(), key.NamespaceId.ToString(), topPermission);
    }

    public async Task<HubToAuthResult> ValidateBasicAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || password.Length < 12)
            return HubToAuthResult.Failure("Invalid credentials");

        var normalizedUsername = username.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByUsernameAsync(normalizedUsername, cancellationToken);

        if (user is null)
        {
            _argon2Helper.Verify(password, "$argon2id$v=19$m=32768,t=3,p=2$6Owth8NxVLmZXdsfKmhrIQ==$h7x1EjXRKSXdluIcBB3teH5vGXY2MFHuPbFoXjxRPt4=");
            return HubToAuthResult.Failure("Username or password is incorrect.");
        }

        var prefix = password.Substring(0, 12);
        var candidates = await _apiKeyRepository.GetByPrefixAsync(prefix, cancellationToken);
        var key = candidates.FirstOrDefault(x => _argon2Helper.Verify(password, x.KeyHash));

        if (key is null || key.UserId != user.Id)
            return HubToAuthResult.Failure("Username or password is incorrect.");

        var topPermission = key.Permission == ApiKeyPermission.Write ? "Write" : "Read";

        return HubToAuthResult.Success(key.UserId.ToString(), key.NamespaceId.ToString(), topPermission);
    }
}
