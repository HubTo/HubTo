using HubTo.Core.Application.Contracts.Persistence.Repositories;
using HubTo.Core.Domain.SeedWork;
using HubTo.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HubTo.Infrastructure.Persistence.Repositories;

internal class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    #region Dependencies
    private readonly HubToContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(HubToContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }
    #endregion

    public async Task<Guid> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var response = await _dbSet.AddAsync(entity, cancellationToken);

        return response.Entity.Id;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.IsDeleted = true;
        _dbSet.Update(entity);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().AnyAsync(predicate, cancellationToken);
    }

    public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public IQueryable<TEntity> Query => _dbSet.AsQueryable();
}