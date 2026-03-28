using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.Entities;
using HubTo.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HubTo.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository : Repository<UserEntity>, IUserRepository
{
    public UserRepository(HubToContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<UserEntity>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        return (await WhereAsync(x => true, cancellationToken)).ToArray();
    }

    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var users = await WhereAsync(x => x.Email == email, cancellationToken);
        return users.FirstOrDefault();
    }

    public async Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = username.Trim().ToLowerInvariant();

        var users = await WhereAsync(x => x.Username.ToLower().Contains(normalizedUsername), cancellationToken);
        return users.FirstOrDefault();
    }

    public async Task<IReadOnlyList<UserEntity>> SearchByEmailOrUsernameAsync(string query, CancellationToken cancellationToken = default)
    {
        var normalized = query.Trim().ToLowerInvariant();

        return await Query
            .AsNoTracking()
            .Where(x => !(x.Email.ToLower().Contains(normalized) || x.Username.ToLower().Contains(normalized)))
            .OrderBy(x => x.Username)
            .Take(8)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await ExistsAsync(x => x.Username == username, cancellationToken);
    }
}
