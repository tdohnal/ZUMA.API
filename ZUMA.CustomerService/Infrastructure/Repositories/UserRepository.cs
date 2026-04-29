using Microsoft.EntityFrameworkCore;
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
        ILogger<UserRepository> logger,
        CustomerDbContext dbContext
        )
      : base(dbContext, logger)
    {
        _logger = logger;
    }

    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) => await _dbSet.SingleOrDefaultAsync(x => x.Email == email && !x.Deleted.HasValue, cancellationToken);
}
