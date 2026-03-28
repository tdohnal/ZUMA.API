using ZUMA.BussinessLogic.Entities;
using ZUMA.BussinessLogic.Repositories;
using ZUMA.BussinessLogic.Services;

namespace ZUMA.DataCleaner.Services
{
    internal class DataCleanerService<T> : ServiceBase<T> where T : IAuditableEntities
    {
        public DataCleanerService(IRepositoryBase<T> repository) : base(repository)
        {
        }

        public virtual async Task CleanDataAsync(CancellationToken cancellationToken = default)
        {
            var thresholdDate = DateTime.UtcNow.AddMonths(-3);

            var queryEntity = _repository.GetQueryable();

            // Teprve tady v C# filtrujeme
            var entitiesToDelete = queryEntity
                .Where(e => e.Deleted.HasValue && e.Deleted.Value <= thresholdDate).ToList();

            foreach (var entity in entitiesToDelete)
            {
                await _repository.DeleteAsync(entity.InternalId, cancellationToken);
            }
        }
    }
}
