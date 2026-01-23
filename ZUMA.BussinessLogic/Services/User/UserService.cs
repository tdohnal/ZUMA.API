using Microsoft.Extensions.Logging;
using ZUMA.BussinessLogic.Infrastructure.Entities.Customer;
using ZUMA.BussinessLogic.Repositories.User;

namespace ZUMA.BussinessLogic.Services.User;

internal class UserService : ServiceBase<UserEntity>, IUserService
{
    private readonly ILogger<UserRepository> _logger;

    public UserService
        (
        IUserRepository userRepository,
        ILogger<UserRepository> logger
        ) : base(userRepository)
    {
        _logger = logger;
    }
}
