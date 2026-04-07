using Zuma.Customer.Domain.Entities;
using ZUMA.SharedKernel.Repositories;

namespace Zuma.Customer.Domain.Interfaces;

public interface IUserRepository : IRepositoryBase<UserEntity>
{
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
