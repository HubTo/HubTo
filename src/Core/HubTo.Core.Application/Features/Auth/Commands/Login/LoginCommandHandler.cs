using HubTo.Core.Application.Common;
using HubTo.Core.Application.Common.CQRS;
using HubTo.Core.Application.Common.Helpers.Argon2Helper;
using HubTo.Core.Application.Common.Helpers.JwtHelper;
using HubTo.Core.Application.Contracts.Persistence.Repositories;

namespace HubTo.Core.Application.Features.Auth.Commands.Login;

internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IArgon2Helper _argon2Helper;
    private readonly IJwtHelper _jwtHelper;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IArgon2Helper argon2Helper,
        IJwtHelper jwtHelper)
    {
        _userRepository = userRepository;
        _argon2Helper = argon2Helper;
        _jwtHelper = jwtHelper;
    }

    public async ValueTask<Result<LoginDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedUsername = request.Username.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByUsernameAsync(normalizedUsername, cancellationToken);

        if (user is null)
        {
            _argon2Helper.Verify(request.Password, "$argon2id$v=19$m=32768,t=3,p=2$6Owth8NxVLmZXdsfKmhrIQ==$h7x1EjXRKSXdluIcBB3teH5vGXY2MFHuPbFoXjxRPt4=");

            return Result<LoginDto>.Fail(new List<string> { "Username or password is incorrect." });
        }

        if (!_argon2Helper.Verify(request.Password, user.PasswordHash))
        {
            return Result<LoginDto>.Fail(new List<string> { "Username or password is incorrect." });
        }

        UserJwtDto userJwtDto = new UserJwtDto(
            user.Id, user.Email, user.Username,
            user.UserNamespaces.Select(ur => (ur.NamespaceId, (int)ur.NamespaceRole)).ToList()
        );

        var (accessToken, expiresAt) = _jwtHelper.GenerateAccessToken(userJwtDto);

        return Result<LoginDto>.Ok(new LoginDto(accessToken, expiresAt));
    }
}
