using ZUMA.BussinessLogic.Entities.Customer;

namespace ZUMA.BussinessLogic.Services.User;

public interface IUserService : IServiceBase<UserEntity>
{
    Task<long?> GetIdByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<string> GetAuthorizationCodeAsync(long id, CancellationToken cancellationToken = default);
}
