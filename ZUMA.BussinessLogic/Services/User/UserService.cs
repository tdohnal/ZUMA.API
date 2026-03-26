using Microsoft.Extensions.Logging;
using ZUMA.BussinessLogic.Entities.Customer;
using ZUMA.BussinessLogic.Repositories.User;
using ZUMA.BussinessLogic.Utils;

namespace ZUMA.BussinessLogic.Services.User;

internal class UserService : ServiceBase<UserEntity>, IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;

    public UserService
        (
        IUserRepository userRepository,
        ILogger<UserService> logger
        ) : base(userRepository)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    protected override Task BeforeCreateAsync(UserEntity entity, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating a new user with username: {Username}", entity.UserName);
        return base.BeforeCreateAsync(entity, cancellationToken);
    }

    public async Task<long?> GetIdByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentNullException(nameof(email));
        }

        try
        {
            var ret = await _userRepository.GetByEmailAsync(email, cancellationToken);
            return ret?.InternalId;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<string> GetAuthorizationCodeAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var ret = await _repository.GetByIdAsync(id, cancellationToken);

            var code = CodeGenerator.GenerateNumericCode(8);
            ret.AuthCode = code;
            ret.AuthCodeExpiration = DateTime.UtcNow.AddMinutes(15);
            await _repository.UpdateAsync(ret, cancellationToken);

            return code;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
