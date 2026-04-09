using ZUMA.CustomerService.Domain.Entities;
using ZUMA.SharedKernel.Repositories;

namespace ZUMA.CustomerService.Domain.Interfaces;

public interface IUserRepository : IRepositoryBase<UserEntity>
{
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
