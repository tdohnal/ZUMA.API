using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zuma.Customer.Domain.Entities;
using Zuma.Customer.Domain.Interfaces;
using ZUMA.Customer.Infrastructure.Persistance;
using ZUMA.SharedKernel.Repositories;

namespace ZUMA.Customer.Infrastructure.Repositories;

internal class RegistrationRepository : RepositoryBase<RegistrationEntity>, IRegistrationRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly CustomerDbContext _dbContext;

    public RegistrationRepository
        (
        ILogger<UserRepository> logger,
        CustomerDbContext dbContext
        )
      : base(dbContext, logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public override async Task<IList<RegistrationEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Registrations
                               .Where(x => !x.Deleted.HasValue)
                               .Include(x => x.UserId)
                               .ToListAsync(cancellationToken);
    }

    public override async Task<RegistrationEntity?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Registrations
                               .Where(x => x.PublicId == publicId && !x.Deleted.HasValue)
                               .Include(x => x.UserId)
                               .FirstOrDefaultAsync(cancellationToken);
    }

    public override async Task<RegistrationEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Registrations
                               .Where(x => x.Id == id && !x.Deleted.HasValue)
                               .Include(x => x.UserId)
                               .FirstOrDefaultAsync(cancellationToken);
    }
}
