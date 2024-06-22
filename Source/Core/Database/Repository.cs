using System.Linq.Expressions;
using Core.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Core.Database;

public interface IRepository
    <TEntity, in TKey>
    where TEntity : IEntity<TKey>
    where TKey : IComparable, IEquatable<TKey>
{
    IQueryable<TEntity> Search(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null
    );

    IQueryable<T> Search<T>(
        Expression<Func<TEntity, T>> select,
        Expression<Func<TEntity, bool>>? filter,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy
    );

    Task<TEntity?> GetSingleAsync(TKey id);

    Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> condition);

    Task InsertAsync(TEntity entity, bool save = true);

    Task DeleteAsync(TEntity entity, bool save = true);

    Task DeleteAsync(TKey id, bool save = true);

    Task SaveAsync();

    Task UpdateAsync(TEntity entity, bool save = true);
}

public abstract class Repository<TEntity, TKey, TContext> : IRepository<TEntity, TKey>
    where TKey : IComparable, IEquatable<TKey>
    where TEntity : class, IEntity<TKey>
    where TContext : DbContext
{
    protected readonly TContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(TContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual IQueryable<TEntity> Search(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null
    )
    {
        IQueryable<TEntity> query = DbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            return orderBy(query);
        }

        return query;
    }

    public virtual IQueryable<T> Search<T>(
        Expression<Func<TEntity, T>> select,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null
    )
    {
        IQueryable<TEntity> query = DbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query.Select(select);
    }

    public virtual async Task<TEntity?> GetSingleAsync(TKey id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> condition)
    {
        return await DbSet.FirstOrDefaultAsync(condition);
    }

    public virtual async Task InsertAsync(TEntity entity, bool save = true)
    {
        entity.CreatedAt = DateTimeOffset.UtcNow;
        EntityEntry<TEntity> entry = DbSet.Entry(entity);
        entry.State = EntityState.Added;

        if (save)
        {
            await Context.SaveChangesAsync();
        }
    }

    public virtual async Task DeleteAsync(TEntity entity, bool save = true)
    {
        if (Context.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }

        DbSet.Remove(entity);

        if (save)
        {
            await Context.SaveChangesAsync();
        }
    }

    public virtual async Task DeleteAsync(TKey id, bool save = true)
    {
        TEntity? entity = await DbSet.FindAsync(id);

        if (entity == null)
        {
            return;
        }

        await DeleteAsync(entity, save);
    }

    public virtual async Task UpdateAsync(TEntity entity, bool save = true)
    {
        DbSet.Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;

        if (save)
        {
            await Context.SaveChangesAsync();
        }
    }

    public virtual async Task SaveAsync()
    {
        await Context.SaveChangesAsync();
    }
}
