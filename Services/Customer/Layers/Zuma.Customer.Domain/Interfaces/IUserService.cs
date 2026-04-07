using Zuma.Customer.Domain.Entities;
using ZUMA.SharedKernel.Services;

namespace Zuma.Customer.Domain.Interfaces;

public interface IUserService : IServiceBase<UserEntity>
{
    Task<long?> GetIdByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task GetAuthorizationCodeAsync(long id, CancellationToken cancellationToken = default);

    Task<VerificationResult> VerificateAuthorizationCode(string code, string email, CancellationToken cancellationToken = default);
}
