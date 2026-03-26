using ZUMA.BussinessLogic.Entities;
using ZUMA.BussinessLogic.Repositories;

namespace ZUMA.BussinessLogic.Services;

public class ServiceBase<T> : IServiceBase<T> where T : IAuditableEntities
{
    protected readonly IRepositoryBase<T> _repository;

    public ServiceBase(IRepositoryBase<T> repository)
    {
        _repository = repository;
    }

    public virtual async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(id, cancellationToken);

    public virtual async Task<T?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default)
        => await _repository.GetByPublicIdAsync(publicId, cancellationToken);

    public virtual async Task<IList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);

    public virtual async Task<T?> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await BeforeCreateAsync(entity, cancellationToken);
        var result = await _repository.CreateAsync(entity, cancellationToken);
        await AfterCreateAsync(result!, cancellationToken);
        return result;
    }

    public virtual async Task<T?> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await BeforeUpdateAsync(entity, cancellationToken);
        var result = await _repository.UpdateAsync(entity, cancellationToken);
        await AfterUpdateAsync(result!, cancellationToken);
        return result;
    }

    public virtual async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await BeforeDeleteAsync(id, cancellationToken);
        var result = await _repository.DeleteAsync(id, cancellationToken);
        await AfterDeleteAsync(id, cancellationToken);
        return result;
    }

    #region Hooks (virtual)

    protected virtual Task BeforeCreateAsync(T entity, CancellationToken cancellationToken)
    {
        entity.Created = DateTime.UtcNow;
        return Task.CompletedTask;
    }
    protected virtual Task AfterCreateAsync(T entity, CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task BeforeUpdateAsync(T entity, CancellationToken cancellationToken)
    {
        entity.Updated = DateTime.UtcNow;
        return Task.CompletedTask;
    }
    protected virtual Task AfterUpdateAsync(T entity, CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task BeforeDeleteAsync(int id, CancellationToken cancellationToken) => Task.CompletedTask;
    protected virtual Task AfterDeleteAsync(int id, CancellationToken cancellationToken) => Task.CompletedTask;

    #endregion
}
