using HubTo.Core.Application.Contracts;
using HubTo.Core.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HubTo.Infrastructure.Persistence.Interceptors;

internal sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IClientContext _clientContext;

    public AuditInterceptor(IClientContext clientContext)
    {
        _clientContext = clientContext;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateAuditEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditEntities(DbContext? context)
    {
        if (context == null) return;

        var userId = _clientContext.UserId ?? Guid.Empty;
        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = userId;
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedBy = userId;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.CreatedBy).IsModified = false;
                entry.Property(x => x.CreatedAt).IsModified = false;
                entry.Entity.UpdatedBy = userId;
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}
