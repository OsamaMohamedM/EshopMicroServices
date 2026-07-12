using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OrderingDomain.Abestrations;

namespace OrderingInfastructure.Data.Interspector
{
    public class AuditTableEntityInterpector : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntites(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntites(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        private void UpdateEntites(DbContext? context)
        {
            if (context == null) return;
            var entries = context.ChangeTracker.Entries<IEntity>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = "Osama";
                }
                if (entry.State == EntityState.Modified || entry.State == EntityState.Added || entry.HasChangedOwnedEntry())
                {
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = "Osama";
                }
            }
        }
    }

    public static class EntityEntryExtensions
    {
        public static bool HasChangedOwnedEntry(this Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            return entry.References.Any(r => r.TargetEntry != null
            && r.TargetEntry.Metadata.IsOwned()
            &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
        }
    }
}