using ZUMA.CustomerService.Domain.Entities;
using ZUMA.SharedKernel.Services;

namespace ZUMA.CustomerService.Domain.Interfaces;

public interface IUserService : IServiceBase<UserEntity>
{
    Task<long?> GetIdByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task GetAuthorizationCodeAsync(long id, CancellationToken cancellationToken = default);

    Task<VerificationResult> VerificateAuthorizationCode(string code, string email, CancellationToken cancellationToken = default);
}
