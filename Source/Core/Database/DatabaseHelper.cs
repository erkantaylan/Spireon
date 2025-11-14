using Core.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Core.Database;


public static class DatabaseHelper
{
    public static void ApplyChanges(DbContext context)
    {
        AddDatetimeWhenCreated(context);
        AddDateWhenModified(context);
        AddSoftDelete(context);
    }
    
    public static void AddDatetimeWhenCreated(DbContext context)
    {
        IEnumerable<object> createdEntires = context.ChangeTracker
                                                    .Entries()
                                                    .Where(x => x.State == EntityState.Added)
                                                    .Select(x => x.Entity);

        foreach (object entry in createdEntires)
        {
            if (entry is IHaveDates auditableEntity)
            {
                auditableEntity.CreatedAt = DateTimeOffset.UtcNow;
            }
        }
    }

    public static void AddDateWhenModified(DbContext context)
    {
        IEnumerable<object> modifiedEntries = context.ChangeTracker
                                                     .Entries()
                                                     .Where(x => x.State == EntityState.Modified)
                                                     .Select(x => x.Entity);

        foreach (object modifiedEntry in modifiedEntries)
        {
            if (modifiedEntry is IHaveDates auditableEntity)
            {
                auditableEntity.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }
    }

    public static void AddSoftDelete(DbContext context)
    {
        IEnumerable<EntityEntry> deletedEntries = context.ChangeTracker
                                                         .Entries()
                                                         .Where(x => x.State == EntityState.Deleted);

        foreach (EntityEntry entry in deletedEntries)
        {
            if (entry.Entity is IHaveDates entity)
            {
                entity.DeletedAt = DateTimeOffset.UtcNow;
                entry.State = EntityState.Modified;
            }
        }
    }
}