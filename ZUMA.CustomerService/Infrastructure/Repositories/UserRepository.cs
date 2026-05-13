using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ZUMA.CustomerService.Domain.Entities;
using ZUMA.CustomerService.Domain.Interfaces;
using ZUMA.CustomerService.Infrastructure.Persistence;
using ZUMA.SharedKernel.Infrastructure.Repositories;

namespace ZUMA.CustomerService.Infrastructure.Repositories;

internal class UserRepository : RepositoryBase<UserEntity>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository
        (
        CustomerDbContext dbContext,
        IDistributedCache cache,
        ILogger<UserRepository> logger

        )
      : base(dbContext, cache, logger)
    {
        _logger = logger;
    }

    protected override bool IsCacheEnabled => true;

    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) => await _dbSet.SingleOrDefaultAsync(x => x.Email == email && !x.Deleted.HasValue, cancellationToken);
}
