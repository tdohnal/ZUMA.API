using Microsoft.Extensions.Logging;
using ZUMA.BussinessLogic.Infrastructure.Entities.Customer;
using ZUMA.BussinessLogic.Repositories.User;

namespace ZUMA.BussinessLogic.Services.User;

internal class UserService : ServiceBase<UserEntity>, IUserService
{
    private readonly ILogger<UserService> _logger;

    public UserService
        (
        IUserRepository userRepository,
        ILogger<UserService> logger
        ) : base(userRepository)
    {
        _logger = logger;
    }

    protected override Task BeforeCreateAsync(UserEntity entity, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating a new user with username: {Username}", entity.UserName);
        return base.BeforeCreateAsync(entity, cancellationToken);
    }
}
