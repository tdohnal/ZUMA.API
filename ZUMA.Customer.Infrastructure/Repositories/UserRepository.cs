using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zuma.Customer.Domain.Entities;
using Zuma.Customer.Domain.Interfaces;
using ZUMA.Customer.Infrastructure.Persistance;
using ZUMA.SharedKernel.Repositories;

namespace ZUMA.Customer.Infrastructure.Repositories;

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

    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) => await _dbSet.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);
}
