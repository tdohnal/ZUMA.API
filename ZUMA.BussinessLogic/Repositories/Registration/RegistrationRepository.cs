using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZUMA.BussinessLogic.Entities.Customer;
using ZUMA.BussinessLogic.Infrastructure.Contexts.Customer;

namespace ZUMA.BussinessLogic.Repositories.User;

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
}
