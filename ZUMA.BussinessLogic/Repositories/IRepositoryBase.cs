using ZUMA.BussinessLogic.Infrastructure.Entities;

namespace ZUMA.BussinessLogic.Repositories;

public interface IRepositoryBase<T> where T : IAuditableEntities
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<T?> CreateAsync(T entity, CancellationToken cancellationToken = default);
}