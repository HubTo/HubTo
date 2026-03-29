using HubTo.Core.Application.Common.CQRS;
using HubTo.Core.Application.Common.Helpers.Argon2Helper;
using HubTo.Core.Application.Common.Results;
using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.Entities;

namespace HubTo.Core.Application.Features.Auth.Commands.Register;

internal sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegisterDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IArgon2Helper _argon2Helper;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IArgon2Helper argon2Helper)
    {
        _userRepository = userRepository;
        _argon2Helper = argon2Helper;
    }

    public async ValueTask<Result<RegisterDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var normalizedUsername = request.Username.Trim();

        var emailTask = await _userRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken);
        if (emailTask)
            errors.Add("Email already exists.");

        var usernameTask = await _userRepository.ExistsByUsernameAsync(normalizedUsername, cancellationToken);

        if (usernameTask)
            errors.Add("Username already exists.");

        if (errors.Any())
            return Result<RegisterDto>.Fail(errors);

        var passwordHash = _argon2Helper.Hash(request.Password);

        var user = new UserEntity
        {
            Email = normalizedEmail,
            Username = request.Username,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.AddAsync(user, cancellationToken);

        return Result<RegisterDto>.Ok(new RegisterDto(user.Id));
    }
}