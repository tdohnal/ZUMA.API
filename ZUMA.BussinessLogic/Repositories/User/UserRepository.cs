using Microsoft.Extensions.Logging;
using ZUMA.BussinessLogic.Infrastructure.Contexts.Customer;
using ZUMA.BussinessLogic.Infrastructure.Entities.Customer;

namespace ZUMA.BussinessLogic.Repositories.User;

internal class UserRepository : RepositoryBase<UserEntity>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly CustomerDbContext _dbContext;

    public UserRepository
        (
        ILogger<UserRepository> logger,
        CustomerDbContext dbContext
        )
      : base(dbContext, logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
}
