using Microsoft.EntityFrameworkCore;
using ZUMA.BussinessLogic.Infrastructure.Entities;

namespace ZUMA.BussinessLogic.Repositories;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class, IAuditableEntities
{
    protected readonly DbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public RepositoryBase(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public virtual async Task<IList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(x => !x.Deleted.HasValue).ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task<T?> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null) return false;

        entity.Deleted = DateTime.UtcNow;
        _dbSet.Attach(entity);
        _dbContext.Entry(entity).Property(x => x.Deleted).IsModified = true;

        var affected = await _dbContext.SaveChangesAsync(cancellationToken);
        return affected > 0;
    }
}
