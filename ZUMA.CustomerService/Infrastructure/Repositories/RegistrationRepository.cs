using Microsoft.EntityFrameworkCore;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.CustomerService.Infrastructure.Persistence;
using ZUMA.SharedKernel.Repositories;

namespace ZUMA.CustomerService.Infrastructure.Repositories;

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

    protected override IQueryable<RegistrationEntity> ApplyIncludes(IQueryable<RegistrationEntity> query)
    {
        return query.Include(x => x.User);
    }
}
