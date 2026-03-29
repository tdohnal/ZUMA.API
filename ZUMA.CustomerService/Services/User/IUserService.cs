using ZUMA.BussinessLogic.Services;
using ZUMA.CustomerService.Entities;

namespace ZUMA.CustomerService.Services.User;

public interface IUserService : IServiceBase<UserEntity>
{
    Task<long?> GetIdByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task GetAuthorizationCodeAsync(long id, CancellationToken cancellationToken = default);

    Task<VerificationResult> VerificateAuthorizationCode(string code, string email, CancellationToken cancellationToken = default);
}
