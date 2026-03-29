using Microsoft.EntityFrameworkCore;
using ZUMA.BussinessLogic.Infrastructure.Contexts.Customer;
using ZUMA.BussinessLogic.Repositories;
using ZUMA.CustomerService.Entities;

namespace ZUMA.CustomerService.Repositories.User;

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
