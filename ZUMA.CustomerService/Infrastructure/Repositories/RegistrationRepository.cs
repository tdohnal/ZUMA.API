using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.CustomerService.Infrastructure.Persistence;
using ZUMA.SharedKernel.Infrastructure.Repositories;

namespace ZUMA.CustomerService.Infrastructure.Repositories;

internal class RegistrationRepository : RepositoryBase<RegistrationEntity>, IRegistrationRepository
{
    private readonly ILogger<RegistrationRepository> _logger;
    private readonly CustomerDbContext _dbContext;

    public RegistrationRepository
        (
          CustomerDbContext dbContext,
          IDistributedCache cache,
          ILogger<RegistrationRepository> logger

        )
      : base(dbContext, cache, logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    protected override bool IsCacheEnabled => true;

    protected override IQueryable<RegistrationEntity> ApplyIncludes(IQueryable<RegistrationEntity> query)
    {
        return query.Include(x => x.User);
    }
}
